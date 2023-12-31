﻿using FI.AtividadeEntrevista.BLL;
using WebAtividadeEntrevista.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FI.AtividadeEntrevista.DML;
using WebAtividadeEntrevista.Controllers.Services;
using System.Web.Http.Results;

namespace WebAtividadeEntrevista.Controllers
{
    public class ClienteController : Controller
    {
        private ServicesCliente _servicesCliente = new ServicesCliente();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Incluir()
        {
            return View();
        }

        [HttpPost]
        public JsonResult Incluir(ClienteModel model)
        {
            Cliente cli = new Cliente();
            BoCliente bo = new BoCliente();
            BoBeneficiarioCliente boB = new BoBeneficiarioCliente();
            string cpfCliente = model.CPF;
            List<string> ss = TempData["CPF"] as List<string>;

            List<BeneficiarioCliente> listaTempBenef = (List<BeneficiarioCliente>)TempData["listaBenef"];

            if (!this.ModelState.IsValid)
            {
                List<string> erros = (from item in ModelState.Values
                                      from error in item.Errors
                                      select error.ErrorMessage).ToList();

                Response.StatusCode = 400;
                return Json(string.Join(Environment.NewLine, erros));
            }
            else if (_servicesCliente.ValidaCpf(cpfCliente) && !bo.VerificarExistencia(cpfCliente))
            {
                if (!bo.VerificarExistencia(cpfCliente))
                {
                    model.Id = bo.Incluir(new Cliente()
                    {
                        CEP = model.CEP,
                        Cidade = model.Cidade,
                        Email = model.Email,
                        Estado = model.Estado,
                        Logradouro = model.Logradouro,
                        Nacionalidade = model.Nacionalidade,
                        Nome = model.Nome,
                        Sobrenome = model.Sobrenome,
                        Telefone = model.Telefone,
                        CPF = model.CPF
                    });

                    int idCliente = (int)model.Id;

                    if (listaTempBenef != null && listaTempBenef.Count > 0)
                    {
                        foreach (var benef in listaTempBenef)
                        {
                            benef.IDCLIENTE = idCliente;
                            boB.Incluir(benef);
                        }
                    }

                    return Json("Cadastro efetuado com sucesso");
                }
                else { return Json("CPF ja cadastrado"); };
            }
            else { return Json("CPF invalido"); };
        }

        [HttpPost]
        public JsonResult Alterar(ClienteModel model)
        {
            string cpfCliente = model.CPF;
            BoCliente bo = new BoCliente();
            BoBeneficiarioCliente boBenef = new BoBeneficiarioCliente();
            List<BeneficiarioCliente> listaTempBenef = (List<BeneficiarioCliente>)TempData["listaBenef"];

            if (!this.ModelState.IsValid)
            {
                List<string> erros = (from item in ModelState.Values
                                      from error in item.Errors
                                      select error.ErrorMessage).ToList();

                Response.StatusCode = 400;
                return Json(string.Join(Environment.NewLine, erros));
            }
            else if (_servicesCliente.ValidaCpf(cpfCliente))
            {

                bo.Alterar(new Cliente()
                {
                    Id = model.Id,
                    CEP = model.CEP,
                    Cidade = model.Cidade,
                    Email = model.Email,
                    Estado = model.Estado,
                    Logradouro = model.Logradouro,
                    Nacionalidade = model.Nacionalidade,
                    Nome = model.Nome,
                    Sobrenome = model.Sobrenome,
                    Telefone = model.Telefone,
                    CPF = model.CPF
                });

                int idCliente = (int)model.Id;

                boBenef.Excluir(idCliente);

                if (listaTempBenef != null && listaTempBenef.Count > 0)
                {
                    foreach (var benef in listaTempBenef)
                    {
                        benef.IDCLIENTE = idCliente;
                        boBenef.Incluir(benef);
                    }
                }

                return Json("Cadastro alterado com sucesso");
            }
            else
            {
                return Json("CPF invalido");
            }
        }

        public void IncluirBeneficiario(BeneficiarioCliente benef)
        {
            BoBeneficiarioCliente boBenef = new BoBeneficiarioCliente();
            boBenef.Incluir(benef);
        }

        [HttpGet]
        public ActionResult Alterar(long id)
        {
            //TODO: Trazer functions da view incluir tb para a view Alterar
            BoCliente bo = new BoCliente();
            BoBeneficiarioCliente benef = new BoBeneficiarioCliente();
            Cliente cliente = bo.Consultar(id);
            Models.ClienteModel model = null;
            List<BeneficiarioCliente> listaBenef = new List<BeneficiarioCliente>();
            listaBenef = benef.Consultar(id);

            if (cliente != null)
            {
                model = new ClienteModel()
                {
                    Id = cliente.Id,
                    CEP = cliente.CEP,
                    Cidade = cliente.Cidade,
                    Email = cliente.Email,
                    Estado = cliente.Estado,
                    Logradouro = cliente.Logradouro,
                    Nacionalidade = cliente.Nacionalidade,
                    Nome = cliente.Nome,
                    Sobrenome = cliente.Sobrenome,
                    Telefone = cliente.Telefone,
                    CPF = cliente.CPF,
                    ListaBenef = listaBenef
                };              
                ViewBag.Cidade = model.Cidade;
                ViewBag.CEP = cliente.CEP;
                ViewBag.Email = cliente.Email;
                ViewBag.Estado = cliente.Estado;
                ViewBag.Logradouro = cliente.Logradouro;
                ViewBag.Nacionalidade = cliente.Nacionalidade;
                ViewBag.Nome = cliente.Nome;
                ViewBag.Sobrenome = cliente.Sobrenome;
                ViewBag.Telefone = cliente.Telefone;
                ViewBag.CPF = cliente.CPF;                  
            }

            TempData["listaBenef"] = model.ListaBenef;

            return View(model);
        }



        [HttpPost]
        public JsonResult ClienteList(int jtStartIndex = 0, int jtPageSize = 0, string jtSorting = null)
        {
            try
            {
                int qtd = 0;
                string campo = string.Empty;
                string crescente = string.Empty;
                string[] array = jtSorting.Split(' ');

                if (array.Length > 0)
                    campo = array[0];

                if (array.Length > 1)
                    crescente = array[1];

                List<Cliente> clientes = new BoCliente().Pesquisa(jtStartIndex, jtPageSize, campo, crescente.Equals("ASC", StringComparison.InvariantCultureIgnoreCase), out qtd);

                //Return result to jTable
                return Json(new { Result = "OK", Records = clientes, TotalRecordCount = qtd });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex });
            }
        }

        /// <summary>
        /// Metodo que adiciona novo usuario a lista de beneficiarios do front
        /// </summary>
        /// <param name="cpf"></param>
        /// <param name="nome"></param>
        /// <returns>cpf e nome do novo usuario</returns>
        public ActionResult AdicionarBeneficiario(string cpf, string nome)
        {
            //TODO: Trazer CPF do cliente tambem para náo permitir que CPF de algum benficiario seja o mesmo do cliente

            List<BeneficiarioCliente> listaBenef = (List<BeneficiarioCliente>)TempData["listaBenef"];
 

            try
            {
                if (listaBenef != null)
                {
                    var checExistBenef = listaBenef.FirstOrDefault(b => b.CPF == cpf);

                    if (checExistBenef == null)
                    {
                        var novoBeneficiario = new BeneficiarioCliente
                        {
                            CPF = cpf,
                            Nome = nome
                        };
                        listaBenef.Add(novoBeneficiario);
                        TempData["listaBenef"] = listaBenef;
                    }
                    else
                    {
                        TempData["listaBenef"] = listaBenef;
                        return Json(new { success = false, cpf = cpf, nome = nome });
                    }
                }
                else
                {
                    var novoBeneficiario = new BeneficiarioCliente
                    {
                        CPF = cpf,
                        Nome = nome
                    };
                    listaBenef.Add(novoBeneficiario);
                    TempData["listaBenef"] = listaBenef;
                }

                return Json(new { success = true, cpf = cpf, nome = nome });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex });
            }

        }
        /// <summary>
        /// Metodo que exclui da lista de beneficiarios
        /// </summary>
        /// <param name="cpf"></param>
        /// <returns>acao de exclusao</returns>
        public ActionResult ExcluirBeneficiario(string cpf)
        {
            if (TempData["listaBenef"] != null)
            {
                List<BeneficiarioCliente> listaBenef = (List<BeneficiarioCliente>)TempData["listaBenef"];

                var excluirBeneficiario = listaBenef.FirstOrDefault(b => b.CPF == cpf);

                if (excluirBeneficiario != null)
                {
                    listaBenef.Remove(excluirBeneficiario);
                    TempData["listaBenef"] = listaBenef;

                    return Json(new { success = true });
                }
            }

            return Json(new { success = false });
        }

        /// <summary>
        /// Metodo que valida se CPF e valido de acordo com regra de digito verificador
        /// </summary>
        /// <param name="cpf"></param>
        /// <returns>Retorna um boleano</returns>
        [HttpGet]
        public ActionResult ValidaCPFDigVeri(string cpf)
        {

            bool isValid = _servicesCliente.ValidaCpf(cpf);

            return Json(isValid, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Metodo que retorna se CPF do cliente ja existe na base
        /// </summary>
        /// <param name="cpf"></param>
        /// <returns>recebe boleano</returns>
        [HttpGet]
        public ActionResult ValidaCPFClienteExis(string cpf)
        {
            BoCliente bo = new BoCliente();
            var isValid = !bo.VerificarExistencia(cpf);

            return Json(isValid, JsonRequestBehavior.AllowGet);
        }

        //TODO: Linkar logicas para verificar na lista
        /// <summary>
        /// Valida se CPF sendo cadastrado ja existe na lista de Beneficiarios de sessao
        /// </summary>
        /// <param name="cpf"></param>
        /// <returns>boleano</returns>
        [HttpGet]
        public ActionResult ValidaCPFBenefExistTempData(string cpf)
        {

            List<BeneficiarioCliente> listaBenef = (List<BeneficiarioCliente>)TempData["listaBenef"];



            if (listaBenef.FirstOrDefault(b => b.CPF == cpf) == null)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(true, JsonRequestBehavior.AllowGet);
            }
        }




    }
}