using Microsoft.AspNetCore.Mvc;
using webapifoto.Models;

namespace webapifoto.Controllers
{
    public class FotoController : Controller
    {
        public static IWebHostEnvironment _environment;

        public FotoController(IWebHostEnvironment environment)
        {
            _environment = environment;
        }


        [HttpPost]
        public async Task<ActionResult<Foto>> PostIndex([FromForm] Foto foto)
        {
            try
            {

                string webRootPath = _environment.WebRootPath;
                string rutaArchivos = Path.Combine(webRootPath, "files");

                if(foto.Archivo.Length > 0)
                {
                    if (!Directory.Exists(rutaArchivos))
                    {
                        Directory.CreateDirectory(rutaArchivos);
                    }
                    using(FileStream fileStream = System.IO.File.Create(Path.Combine(rutaArchivos, foto.Archivo.FileName)))
                    {
                        await foto.Archivo.CopyToAsync(fileStream);
                        fileStream.Flush();
                    }
                    foto.Url = $"{Request.Scheme}://{Request.Host}{Request.PathBase}/files/" + foto.Archivo.FileName;
                }
            }
            catch(Exception ex)
            {
                return Problem(ex.Message + _environment.WebRootPath);
            }

            return CreatedAtAction(nameof(PostIndex), new { foto.Nombre, foto.Url });
        }
    }
}
