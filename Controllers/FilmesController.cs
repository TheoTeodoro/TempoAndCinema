using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using TempoAndCinema.Data;
using TempoAndCinema.Models;

namespace TempoAndCinema.Controllers
{
    public class FilmesController : Controller
    {
        private readonly IFilmeRepository _repo;

        public FilmesController(IFilmeRepository repo)
        {
            _repo = repo;
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
            
            
            if (!string.IsNullOrWhiteSpace(filme.ElencoPrincipal))
            {
                var lista = filme.ElencoPrincipal
                    .Split(",", StringSplitOptions.RemoveEmptyEntries)
                    .Select(x => x.Trim())
                    .ToList();

                filme.ElencoPrincipal = JsonSerializer.Serialize(lista);
            }


            filme.DataAtualizacao = DateTime.Now;
            filme.NotaMedia = double.TryParse(
                filme.NotaMedia.ToString().Replace(",", "."),
                System.Globalization.NumberStyles.Any,
                System.Globalization.CultureInfo.InvariantCulture,
                out var nota
            ) ? nota : 0;

            await _repo.UpdateAsync(filme);
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

    }
}
