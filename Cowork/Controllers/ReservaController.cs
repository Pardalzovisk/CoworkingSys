using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Cowork.Data;
using Cowork.Models;

namespace Cowork.Controllers
{
    public class ReservaController : Controller
    {
        private readonly CoworkContext _context;

        public ReservaController(CoworkContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var reservas = _context.Reservas.Include(r => r.Cliente).Include(r => r.Sala);
            return View(await reservas.ToListAsync());
        }

        public IActionResult Create()
        {
            var reserva = new Reserva
            {
                Funcionarios = _context.Funcionarios.ToList()
            };
            ViewBag.SalaId = new SelectList(_context.Salas, "Id", "Nome");
            ViewBag.ClienteId = new SelectList(_context.Clientes, "Id", "Nome");
            return View(reserva);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Reserva reserva, int[] funcionariosSelecionados)
        {
            if (ModelState.IsValid)
            {
                reserva.Funcionarios = new List<Funcionario>();
                foreach (var funcionarioId in funcionariosSelecionados)
                {
                    var funcionario = _context.Funcionarios.Find(funcionarioId);
                    if (funcionario != null)
                    {
                        reserva.Funcionarios.Add(funcionario);
                    }
                }

                _context.Add(reserva);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.SalaId = new SelectList(_context.Salas, "Id", "Nome", reserva.SalaId);
            ViewBag.ClienteId = new SelectList(_context.Clientes, "Id", "Nome", reserva.ClienteId);
            return View(reserva);
        }


        public IActionResult Edit(int id)
        {
            var reserva = _context.Reservas
                .Include(r => r.Funcionarios)
                .FirstOrDefault(r => r.Id == id);

            if (reserva == null)
            {
                return NotFound();
            }

            ViewBag.SalaId = new SelectList(_context.Salas, "Id", "Nome", reserva.SalaId);
            ViewBag.ClienteId = new SelectList(_context.Clientes, "Id", "Nome", reserva.ClienteId);
            ViewBag.Funcionarios = _context.Funcionarios.ToList();

            return View(reserva);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Reserva reserva, int[] funcionariosSelecionados)
        {
            if (id != reserva.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var reservaExistente = _context.Reservas
                        .Include(r => r.Funcionarios)
                        .FirstOrDefault(r => r.Id == id);

                    if (reservaExistente == null)
                    {
                        return NotFound();
                    }

                    // Atualizar propriedades da reserva
                    reservaExistente.DataReserva = reserva.DataReserva;
                    reservaExistente.HorarioInicio = reserva.HorarioInicio;
                    reservaExistente.HorarioFim = reserva.HorarioFim;
                    reservaExistente.ClienteId = reserva.ClienteId;
                    reservaExistente.SalaId = reserva.SalaId;

                    // Atualizar lista de funcionários
                    reservaExistente.Funcionarios.Clear();
                    foreach (var funcionarioId in funcionariosSelecionados)
                    {
                        var funcionario = _context.Funcionarios.Find(funcionarioId);
                        if (funcionario != null)
                        {
                            reservaExistente.Funcionarios.Add(funcionario);
                        }
                    }

                    _context.Update(reservaExistente);
                    _context.SaveChanges();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Reservas.Any(r => r.Id == reserva.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            ViewBag.SalaId = new SelectList(_context.Salas, "Id", "Nome", reserva.SalaId);
            ViewBag.ClienteId = new SelectList(_context.Clientes, "Id", "Nome", reserva.ClienteId);
            ViewBag.Funcionarios = _context.Funcionarios.ToList();

            return View(reserva);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var reserva = await _context.Reservas
                .Include(r => r.Cliente)
                .Include(r => r.Sala)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (reserva == null)
                return NotFound();

            return View(reserva);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var reserva = await _context.Reservas.FindAsync(id);
            if (reserva == null)
            {
                return NotFound();
            }
            _context.Reservas.Remove(reserva);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reserva = await _context.Reservas
                .Include(r => r.Cliente)
                .Include(r => r.Sala)
                .Include(r => r.Funcionarios)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (reserva == null)
            {
                return NotFound();
            }

            return View(reserva);
        }
    }
}
