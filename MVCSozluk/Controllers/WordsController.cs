using BLL;
using Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MVCSozluk.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class WordsController : Controller
    {
        UnitOfWork _uw = new UnitOfWork();
        // GET: Words
        public ActionResult Index(int? langid, int? sil)
        {
            if (sil.HasValue)
            {
                _uw.Words.Delete(sil.Value);
                _uw.Complete();
                return RedirectToAction("Index", new { @langid = langid });
            }

            var list = _uw.Languages.GetAll()
                   .Select(x => new SelectListItem
                   {
                       Text = x.Name,
                       Value = x.Id.ToString()
                   }).ToList();

            list.Insert(0, new SelectListItem
            {
                Text = "Seçiniz",
                Value = ""
            });
            ViewBag.Langs = list;
            if (langid.HasValue)
            {
                ViewBag.langid = langid.Value;
                var wordlist = _uw.Words.Search(x => x.LanguageId == langid.Value);
                return View(wordlist);
            }
            return View();
        }

        [HttpGet]
        public ActionResult Create()
        {
            //ViewBag.LangOptions = _uw.Languages.GetAll();
            //ViewData["LangOptions"] = _uw.Languages.GetAll();
            TempData["LangOptions"] = _uw.Languages
                .GetAll()
                .Select(x => new SelectListItem
                {
                    Text = x.Name,
                    Value = x.Id.ToString()
                });

            return View();
        }

        [HttpPost]
        public ActionResult Create(string WordTxt, int LanguageId)
        {
            if (string.IsNullOrEmpty(WordTxt))
                ModelState.AddModelError("", "Kelime boş bırakılamaz");
            if (ModelState.IsValid)
            {
                Word w = new Word();
                w.WordTxt = WordTxt;
                w.LanguageId = LanguageId;

                var langs = _uw.Languages.GetAll();

                w.Translations = new List<Word>();
                foreach (var item in langs)
                {
                    string input = Request.Form["ceviri" + item.Id];

                    string[] words = input.Split(',');

                    foreach (string a in words)
                    {
                        if (!string.IsNullOrEmpty(a))
                        {
                            Word ceviri = new Word();
                            ceviri.LanguageId = item.Id;
                            ceviri.WordTxt = a;
                            w.Translations.Add(ceviri);
                        }
                    }

                }

                _uw.Words.Add(w);
                _uw.Complete();
                return RedirectToAction("Index");
            }
            return View();

        }

        public JsonResult AutoComplete(int langid, string q)
        {
            var list = _uw.Words.Search(x => x.LanguageId == langid && x.WordTxt.ToLower().StartsWith(q.ToLower())).Select(x => x.WordTxt).ToList();
            return Json(list, JsonRequestBehavior.AllowGet);
        }

    }
}