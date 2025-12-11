using System.Text.Json;
using System.Collections.Generic;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using TempoAndCinema.Data;
using TempoAndCinema.Service;
using TempoAndCinema.Models;
using TempoAndCinema.Service.Weather;
using System.Text;
using TempoAndCinema.Dtos;

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
		private string CsvEscape(string? input)
		{
			if (string.IsNullOrEmpty(input))
				return "";

			// Dobra aspas internas
			input = input.Replace("\"", "\"\"");

			// Aplica norma RFC-4180
			return $"\"{input}\"";
		}


		[HttpGet]
		public async Task<IActionResult> ExportCsv(int id)
		{
			try
			{
				var filme = await _repo.GetByIdAsync(id);
				if (filme == null)
					return NotFound();

				// --- IMAGENS ---
				List<string> imagens;
				try
				{
					imagens = string.IsNullOrEmpty(filme.BackdropsJson)
						? new List<string>()
						: JsonSerializer.Deserialize<List<string>>(filme.BackdropsJson);
				}
				catch
				{
					imagens = new List<string>();
				}

				// --- ELENCO ---
				List<string> elenco;
				try
				{
					elenco = string.IsNullOrEmpty(filme.ElencoPrincipal)
						? new List<string>()
						: JsonSerializer.Deserialize<List<string>>(filme.ElencoPrincipal);
				}
				catch
				{
					elenco = filme.ElencoPrincipal?
									.Split(",", StringSplitOptions.RemoveEmptyEntries)
									.Select(x => x.Trim())
									.ToList()
							   ?? new List<string>();
				}


				// --- CLIMA ---
				WeatherDto? clima = null;

				if (filme.Latitude.HasValue && filme.Longitude.HasValue)
				{
					clima = await _weather.GetWeatherAsync(filme.Latitude.Value, filme.Longitude.Value);
				}

				var weatherDate = clima?.Daily?.Time?.FirstOrDefault();
				var weatherMax = clima?.Daily?.Temperature_2m_Max?.FirstOrDefault();
				var weatherMin = clima?.Daily?.Temperature_2m_Min?.FirstOrDefault();

				// --- SANITIZAR SINOPSE ---
				var sinopseLimpa = filme.Sinopse?
					.Replace("\"", "'")
					.Replace("\r", " ")
					.Replace("\n", " ");

				// --- CSV ---
				var sb = new StringBuilder();

				sb.AppendLine("Id,Titulo,TituloOriginal,Genero,Lingua,Duracao,DataLancamento,NotaMedia,Latitude,Longitude,CidadeReferencia,Sinopse,ElencoPrincipal,WeatherDate,WeatherTempMax,WeatherTempMin");

				var linha = string.Join(",", new[]
				{
	CsvEscape(filme.Id.ToString()),
	CsvEscape(filme.Titulo),
	CsvEscape(filme.TituloOriginal),
	CsvEscape(filme.Genero),
	CsvEscape(filme.Lingua),
	CsvEscape(filme.Duracao.ToString()),
	CsvEscape(filme.DataLancamento?.ToString("yyyy-MM-dd")),
	CsvEscape(filme.NotaMedia?.ToString(System.Globalization.CultureInfo.InvariantCulture)),
	CsvEscape(filme.Latitude?.ToString(System.Globalization.CultureInfo.InvariantCulture)),
	CsvEscape(filme.Longitude?.ToString(System.Globalization.CultureInfo.InvariantCulture)),
	CsvEscape(filme.CidadeReferencia),
	CsvEscape(sinopseLimpa),
	CsvEscape(string.Join("; ", elenco)),
	CsvEscape(weatherDate?.ToString()),
	CsvEscape(weatherMax?.ToString()),
	CsvEscape(weatherMin?.ToString())
});

				sb.AppendLine(linha);

				var bom = Encoding.UTF8.GetPreamble();
				var content = Encoding.UTF8.GetBytes(sb.ToString());
				var bytes = bom.Concat(content).ToArray();

				return File(bytes, "text/csv", $"{filme.Titulo}_export.csv");

			}

			catch (OperationCanceledException) // <--- Captura a exceção de cancelamento

			{
				// O cliente cancelou a requisição. Apenas loga e retorna.
				// O ASP.NET Core irá lidar com o encerramento da conexão.
				Console.WriteLine("Download de CSV cancelado pelo cliente.");
				return new EmptyResult(); // Retorna um resultado vazio ou NoContent()
			}
			catch (Exception ex)
			{
				// Captura outras exceções não esperadas
				Console.WriteLine($"Erro inesperado durante o ExportCsv: {ex}");
				throw; // Re-lança para que o middleware de erro possa lidar, se necessário
			}

		}


	}
}
