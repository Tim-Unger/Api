﻿using Newtonsoft.Json;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Api;

namespace Api.Controllers
{
    public class NattrakController : Controller
    {
        [HttpGet("nattracks")]
        //https://github.com/aogden41/NAT-Tracks
        public JsonResult GetTracks(string id, bool si = false)
        {
            // Return specific track
            if (id != null)
            {
                var charID = id.ToString().ToUpper().ToCharArray()[0];

                if (si) return Json(GetNatTracks.ParseTracks(si).Where(t => t.Id == charID).FirstOrDefault());

                else return Json(GetNatTracks.ParseTracks().Where(t => t.Id == charID).FirstOrDefault());
            }

            // Return metres
            if (si) return Json(GetNatTracks.ParseTracks(si));

            else return Json(GetNatTracks.ParseTracks());
        }
    }
}