using System.Text.Json;
using System.Collections.Generic;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using TempoAndCinema.Data;
using TempoAndCinema.Service;
using TempoAndCinema.Models;
using TempoAndCinema.Service.Weather;

namespace TempoAndCinema.Controllers
{
    public class FilmesController : Controller
    {
        private readonly IFilmeRepository _repo;
		private readonly IWeatherApiService _weather;

		public FilmesController(IFilmeRepository repo, IWeatherApiService weather)
		{
			_repo = repo;
			_weather = weather;
		}

		// GET: /Filmes
		public async Task<IActionResult> Index(string message = null)
        {
            var filmes = await _repo.GetAllAsync();
            ViewBag.Message = message;
            return View(filmes);
        }
        
        // GET: /Filmes/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var filme = await _repo.GetByIdAsync(id);
            if (filme == null)
                return NotFound();

            // -----------------------------
            // Backdrops (com safe parsing)
            // -----------------------------
            List<string> imagens;
            try
            {
                imagens = string.IsNullOrEmpty(filme.BackdropsJson)
                    ? new List<string>()
                    : JsonSerializer.Deserialize<List<string>>(filme.BackdropsJson);
            }
            catch
            {
                // caso esteja salvo como texto simples (não deveria, mas pode acontecer)
                imagens = new List<string>();
            }

            // -----------------------------
            // Elenco (tratamento completo)
            // -----------------------------
            List<string> elenco;

            try
            {
                elenco = string.IsNullOrEmpty(filme.ElencoPrincipal)
                    ? new List<string>()
                    : JsonSerializer.Deserialize<List<string>>(filme.ElencoPrincipal);
            }
            catch
            {
                // SE NÃO É JSON → ENTÃO É CSV
                elenco = filme.ElencoPrincipal?
                             .Split(",", StringSplitOptions.RemoveEmptyEntries)
                             .Select(x => x.Trim())
                             .ToList()
                         ?? new List<string>();
            }

            ViewBag.Imagens = imagens;
            ViewBag.ElencoCompleto = elenco;

			// CLIMA 
			if (filme.Latitude.HasValue && filme.Longitude.HasValue)
			{
				var clima = await _weather.GetWeatherAsync(
					filme.Latitude.Value,
					filme.Longitude.Value
				);

                ViewBag.Weather = clima;
			}
			else
			{
				ViewBag.Weather = null;
			}


			return View(filme);
        }



        // GET: /Filmes/Create
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Filme filme)
        {
            Console.WriteLine("ENTER Create POST");

            if (!ModelState.IsValid)
            {
                var errors = string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                Console.WriteLine("ModelState inválido: " + errors);
                // adiciona um erro visível para o usuário também
                ModelState.AddModelError(string.Empty, "Dados inválidos: " + errors);
                return View(filme);
            }
            
            if (!string.IsNullOrWhiteSpace(filme.ElencoPrincipal))
            {
                var lista = filme.ElencoPrincipal
                    .Split(",", StringSplitOptions.RemoveEmptyEntries)
                    .Select(x => x.Trim())
                    .ToList();

                filme.ElencoPrincipal = JsonSerializer.Serialize(lista);
            }


            filme.DataCriacao = DateTime.Now;
            filme.DataAtualizacao = DateTime.Now;

            try
            {
                Console.WriteLine("Tentando AddAsync...");
                var id = await _repo.AddAsync(filme);
                Console.WriteLine($"AddAsync retornou id = {id}");
                return RedirectToAction(nameof(Index), new { message = "Filme cadastrado com sucesso!" });
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception ao salvar filme: " + ex);
                ModelState.AddModelError(string.Empty, "Erro ao salvar: " + ex.Message);
                return View(filme);
            }
        }


        // GET: /Filmes/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var filme = await _repo.GetByIdAsync(id);
            if (filme == null)
                return NotFound();
            if (!string.IsNullOrEmpty(filme.ElencoPrincipal))
            {
                try
                {
                    var list = JsonSerializer.Deserialize<List<string>>(filme.ElencoPrincipal);
                    filme.ElencoPrincipal = string.Join(", ", list);
                }
                catch
                {
                    // Se algum filme antigo estiver salvo errado, apenas deixa como está
                }
            }

            return View(filme);
        }

        // POST: /Filmes/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Filme filme)
        {
            if (!ModelState.IsValid)
                return View(filme);
            
            
            // Carrega o registro atual
            var atual = await _repo.GetByIdAsync(filme.Id);
            if (atual == null) return NotFound();

            // ----------- Atualiza campos simples -----------

            atual.Titulo = filme.Titulo;
            atual.TituloOriginal = filme.TituloOriginal;
            atual.Sinopse = filme.Sinopse;
            atual.DataLancamento = filme.DataLancamento;
            atual.Genero = filme.Genero;
            atual.PosterPath = filme.PosterPath;
            atual.Lingua = filme.Lingua;
            atual.Duracao = filme.Duracao;
            atual.TrailerUrl = filme.TrailerUrl;

            // ----------- Elenco (CSV → JSON seguro) -----------
            if (!string.IsNullOrWhiteSpace(filme.ElencoPrincipal))
            {
                var lista = filme.ElencoPrincipal
                    .Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(x => x.Trim())
                    .Where(x => !string.IsNullOrWhiteSpace(x))
                    .Distinct()
                    .ToList();

                atual.ElencoPrincipal = JsonSerializer.Serialize(lista);
            }
            
            // ----------- Trailer URL (corrige automaticamente o link do YouTube) -----------
            if (!string.IsNullOrWhiteSpace(filme.TrailerUrl))
            {
                string url = filme.TrailerUrl.Trim();

                // Converte YouTube watch → embed
                if (url.Contains("watch?v="))
                {
                    url = url.Replace("watch?v=", "embed/")
                        .Split('&')[0];
                }

                // Atribui corretamente ao objeto salvo no banco!
                atual.TrailerUrl = url;
            }


            // ----------- Latitude / Longitude: conversão segura -----------

            string? latStr = Request.Form["Latitude"].FirstOrDefault();
            string? lonStr = Request.Form["Longitude"].FirstOrDefault();

            if (!string.IsNullOrWhiteSpace(latStr))
            {
                if (double.TryParse(latStr.Replace(",", "."), System.Globalization.NumberStyles.Any,
                    System.Globalization.CultureInfo.InvariantCulture, out var lat))
                {
                    atual.Latitude = lat;
                }
            }

            if (!string.IsNullOrWhiteSpace(lonStr))
            {
                if (double.TryParse(lonStr.Replace(",", "."), System.Globalization.NumberStyles.Any,
                    System.Globalization.CultureInfo.InvariantCulture, out var lon))
                {
                    atual.Longitude = lon;
                }
            }

            // ----------- NotaMedia (não zera se vier vazia) -----------

            string? notaStr = Request.Form["NotaMedia"].FirstOrDefault();

            if (!string.IsNullOrWhiteSpace(notaStr))
            {
                if (double.TryParse(notaStr.Replace(",", "."), System.Globalization.NumberStyles.Any,
                    System.Globalization.CultureInfo.InvariantCulture, out var notaParsed))
                {
                    atual.NotaMedia = notaParsed;
                }
            }
            
           


            // ----------- Auditoria -----------
            atual.DataAtualizacao = DateTime.Now;

            await _repo.UpdateAsync(atual);

            return RedirectToAction(nameof(Index), new { message = "Filme editado com sucesso!" });
        }

        // GET: /Filmes/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var filme = await _repo.GetByIdAsync(id);
            if (filme == null)
                return NotFound();

            return View(filme);
        }

        // POST: /Filmes/DeleteConfirmado
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _repo.DeleteAsync(id);
            return RedirectToAction(nameof(Index), new { message = "Filme excluído com sucesso!" });
        }

        
        [HttpGet]
        public async Task<IActionResult> DebugInsert()
        {
            var f = new Filme
            {
                Titulo = "Teste Quick",
                TituloOriginal = "Teste Quick",
                Sinopse = "Sinopse teste",
                Genero = "Teste",
                PosterPath = "https://example.com/poster.jpg",
                Lingua = "pt",
                Duracao = 90,
                NotaMedia = 7.5,
                DataCriacao = DateTime.Now,
                DataAtualizacao = DateTime.Now
            };

            try
            {
                var id = await _repo.AddAsync(f);
                return Content($"Inserido id={id}");
            }
            catch (Exception ex)
            {
                return Content("Erro: " + ex.ToString());
            }
        }

		[HttpGet]
		public async Task<IActionResult> ExportCsv(int id)
		{
			var filme = await _repo.GetByIdAsync(id);
			if (filme == null)
				return NotFound();

			// monta CSV básico
			var linhas = new List<string>();

			linhas.Add("Campo,Valor");

			linhas.Add($"Título,{filme.Titulo}");
			linhas.Add($"Título Original,{filme.TituloOriginal}");
			linhas.Add($"Gênero,{filme.Genero}");
			linhas.Add($"Sinopse,\"{filme.Sinopse}\"");
			linhas.Add($"Data de Lançamento,{filme.DataLancamento?.ToString("yyyy-MM-dd")}");
			linhas.Add($"Idioma,{filme.Lingua}");
			linhas.Add($"Duração,{filme.Duracao}");
			linhas.Add($"Nota Média,{filme.NotaMedia}");
			linhas.Add($"Poster,{filme.PosterPath}");
			linhas.Add($"Cidade Referência,{filme.CidadeReferencia}");
			linhas.Add($"Latitude,{filme.Latitude}");
			linhas.Add($"Longitude,{filme.Longitude}");

			// Converte CSV para bytes
			var csv = string.Join("\n", linhas);
			var bytes = System.Text.Encoding.UTF8.GetBytes(csv);

			// Baixar arquivo
			return File(bytes, "text/csv", $"filme_{filme.Id}.csv");
		}

	}
}
