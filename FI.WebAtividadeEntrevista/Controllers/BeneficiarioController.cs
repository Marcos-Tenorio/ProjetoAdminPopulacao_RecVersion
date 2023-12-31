﻿using FI.AtividadeEntrevista.BLL;
using WebAtividadeEntrevista.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FI.AtividadeEntrevista.DML;
using WebAtividadeEntrevista.Controllers.Services;

namespace WebAtividadeEntrevista.Controllers
{
    public class BeneficiarioController : Controller
    {
        private ServicesCliente _servicesCliente = new ServicesCliente();

        public ActionResult Index()
        {
            return View();
        }
 
        [HttpPost]
        public ActionResult Incluir(BeneficiarioModel beneficiario)
        {
            Models.BeneficiarioModel model = null;

            model = new BeneficiarioModel()
            {
                CPF = beneficiario.CPF,
                Nome = beneficiario.Nome
                
            };


            return View(model);

           
        }



      
        public ActionResult BeneficiarioModel()
        {
            List<BeneficiarioModel> beneficiarioModelList = new List<BeneficiarioModel>();
            {
                new BeneficiarioModel
                {
                    Nome = "",
                    CPF = ""
                };

            };

            ViewBag.List = beneficiarioModelList;
            ViewData["beneficiarioModelList"] = beneficiarioModelList;

            return View();
        }


        /// <summary>
        /// Metodo para excluir beneficiario de uma lista pelo CPF
        /// </summary>
        /// <param name="cpf">CPF presente na linha do beneficiario que foi selecionado no front</param>
        /// <returns></returns>
        public JsonResult ExcluirBeneficiario(string cpf)
        {
            List<BeneficiarioCliente> listaBenef = new List<BeneficiarioCliente>();

            var beneficiarioParaExcluir = listaBenef.FirstOrDefault(b => b.CPF == cpf);

            if (beneficiarioParaExcluir != null)
            {
                listaBenef.Remove(beneficiarioParaExcluir);
                TempData["listaBenef"] = listaBenef;
                return Json(new { success = true });
            }

            return Json(new { success = false });
        }

        [HttpPost]
        public ActionResult ArmazenarBeneficiario(string CPFBenef, string NomeBenef) 
        {            
            TempData["CPFBenef"] = CPFBenef;
            TempData["NomeBenef"] = NomeBenef;

            return new EmptyResult();
        }


    }
}