using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using FavoriteHero.Models;
using FavoriteHero.MarvelAPI;

namespace FavoriteHero.Controllers
{
    public class HomeController : Controller
    {


        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }
        public async Task<IActionResult> Hero()
        {
            var _marvel = new MarvelAPI.Marvel();
            CharacterDataWrapper CharacterModel = await _marvel.GetCharacters("Deadpool");
            Character _character = CharacterModel.Data.Results.FirstOrDefault();

            ComicDataWrapper comicModel = await _marvel.GetComicsForCharacter(_character.Id);
            List<Comic> _comics = comicModel.Data.Results;
            Comic _ComicWithImageDescription = new Comic();
            
           

           
            //some of the data is not in the api db. So skip the comics that do not have images and trim the description
            foreach (var comic in _comics)
            {
                int pathlength = 0;
               



                pathlength = comic.Thumbnail.Path.Length;

                if (comic.Thumbnail.Path.Substring(pathlength - 19, 19) != "image_not_available")
                {
                    if (!String.IsNullOrEmpty(comic.Description))
                    {
                        _ComicWithImageDescription = comic;

                        break;
                    }

                }
            }
            CharacterDataWrapper CharactersModel2 = await _marvel.GetCharactersForComic(_ComicWithImageDescription.Id);
            List<Character> Characters = CharactersModel2.Data.Results;





            CharacterComicViewModel ccVM = new CharacterComicViewModel();

            ccVM.Character = _character;
            ccVM.Comic = _ComicWithImageDescription;
            ccVM.AttributionText = CharacterModel.AttributionText;
            ccVM.Characters = Characters;

            return View(ccVM);

        }



        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

    }
}
