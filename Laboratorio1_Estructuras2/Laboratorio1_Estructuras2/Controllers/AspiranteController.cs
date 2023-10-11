using Microsoft.AspNetCore.Mvc;
using Laboratorio1_Estructuras2.Models;
using System.IO;
using System.Web;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace Laboratorio1_Estructuras2.Controllers
{
    public class AspiranteController : Controller
    {
        public static AVL arbol = new AVL();
        public static List<Aspirante> listaAspi = new List<Aspirante>();
        public static ArbolHuffman huffman = new ArbolHuffman();
        public static ArbolIn confidencial = new ArbolIn();
        public IActionResult Index()
        {
            huffman = huffman.arbol("234987 81324QWERTYUIOP,YUEQIUOQKJADSF-JNVCCHU IEW9413278901-234567890 1 2345ASD,FGHJKLÑHK ADSFHIOEWHE,WFHDSFJBKV-JVCUHA678908,793789347897 8954648798ZXCVBNMJAWFJK'ADSFJIODA  SKNLVADN,J79869");
            listaAspi.Clear();
            return View("Index");
        }
        [Route("carga")]
        public IActionResult SubirDatos(IFormFile archivo)
        {
            if (archivo == null || archivo.Length == 0)
            {
                ViewBag.Error = "Seleccione un archivo CSV válido.";
                return View("SubirDatos", listaAspi);
            }
            using (var reader = new StreamReader(archivo.OpenReadStream()))
            {
                if (listaAspi.Count() > 0)
                {
                    listaAspi.Clear();
                }
                if (arbol.raiz != null)
                {
                    arbol.raiz = null;
                }
                while (!reader.EndOfStream)
                {
                    var linea = reader.ReadLine();
                    var partes = linea.Split(';');
                    if (partes.Length != 2)
                    {
                        continue;
                    }
                    var instruccion = partes[0].Trim();
                    var json = partes[1].Trim();
                    try
                    {
                        JObject jsonData = JObject.Parse(json);
                        var idents = new string[3];
                        if (jsonData.TryGetValue("companies", out JToken companiesToken) && companiesToken.Type == JTokenType.Array)
                        {
                            idents = companiesToken.ToObject<string[]>();
                        }
                        string empres;
                        empres = convertidorVec(idents);
                        Aspirante aspirante = new Aspirante
                        {
                            nombre = (string)jsonData["name"],
                            infoPriv = new List<string> { (string)jsonData["dpi"] },
                            nacimiento = (string)jsonData["datebirth"],
                            direccion = (string)jsonData["address"],
                        };
                        aspirante.infoPriv.Add(empres);
                        switch (instruccion.ToUpper())
                        {
                            case "INSERT":
                                arbol.Insertar(aspirante);
                                break;

                            case "DELETE":
                                arbol.BuscaElimina(aspirante);
                                break;

                            case "PATCH":
                                arbol.actual(aspirante);
                                break;

                            default:
                                return View("Error");
                        }
                    }
                    catch (JsonReaderException)
                    {
                        return View("Error");
                    }
                }
                listaAspi = arbol.listaOrdenada();
            }
            return RedirectToAction("SubirDatos");
        }
        [Route("carga2")]
        public IActionResult SubirDatos2(IFormFile archivo)
        {
            if (archivo == null || archivo.Length == 0)
            {
                ViewBag.Error = "Seleccione un archivo CSV válido.";
                return View("SubirDatos2", listaAspi);
            }
            using (var reader = new StreamReader(archivo.OpenReadStream()))
            {
                if (listaAspi.Count() > 0)
                {
                    listaAspi.Clear();
                }
                if (confidencial.raiz != null)
                {
                    confidencial.raiz = null;
                }
                while (!reader.EndOfStream)
                {
                    var linea = reader.ReadLine();
                    var partes = linea.Split(';');
                    if (partes.Length != 2)
                    {
                        continue;
                    }
                    var instruccion = partes[0].Trim();
                    var json = partes[1].Trim();
                    try
                    {
                        JObject jsonData = JObject.Parse(json);
                        var idents = new string[3];
                        if (jsonData.TryGetValue("companies", out JToken companiesToken) && companiesToken.Type == JTokenType.Array)
                        {
                            idents = companiesToken.ToObject<string[]>();
                        }
                        string empres;
                        empres = convertidorVec(idents);
                        Aspirante aspirante = new Aspirante
                        {
                            nombre = (string)jsonData["name"],
                            infoPriv = new List<string> { (string)jsonData["dpi"] },
                            nacimiento = (string)jsonData["datebirth"],
                            direccion = (string)jsonData["address"],
                        };
                        aspirante.infoPriv.Add(empres);
                        switch (instruccion.ToUpper())
                        {
                            case "INSERT":
                                aspirante.infoPriv[0] = huffman.Codificar(aspirante.infoPriv[0], huffman);
                                aspirante.infoPriv[1] = huffman.Codificar(aspirante.infoPriv[1].ToUpper(), huffman);  
                                confidencial.Insertar(aspirante);
                                break;

                            case "DELETE":
                                aspirante.infoPriv[0] = huffman.Codificar(aspirante.infoPriv[0], huffman);
                                aspirante.infoPriv[1] = huffman.Codificar(aspirante.infoPriv[1].ToUpper(), huffman);
                                confidencial.Eliminar(aspirante);
                                break;

                            case "PATCH":
                                aspirante.infoPriv[0] = huffman.Codificar(aspirante.infoPriv[0], huffman);
                                aspirante.infoPriv[1] = huffman.Codificar(aspirante.infoPriv[1].ToUpper(), huffman);
                                confidencial.actual(aspirante);
                                break;

                            default:
                                return View("Error");
                        }
                    }
                    catch (JsonReaderException)
                    {
                        return View("Error");
                    }
                }
                listaAspi = confidencial.listaOrdenada();
            }
            return RedirectToAction("SubirDatos2");
        }
        [HttpPost]
        [Route("buscarNom")]
        public IActionResult encontrado(string nombre)
        {
            if (nombre == null)
            {
                return View("ErrorBuscar");
            }
            List<Aspirante> aspirantesEncontrados = new List<Aspirante>();
            aspirantesEncontrados = arbol.busqueda(nombre);
            if (aspirantesEncontrados == null)
            {
                return View("ErrorBuscar");
            }
            return View("Encontrado", aspirantesEncontrados);
        }
        [HttpPost]
        [Route("buscarDpi")]
        public IActionResult encontradoDpi(string nombre)
        {
            if (nombre == null)
            {
                return View("ErrorBuscarDpi");
            }
            string codigo = huffman.Codificar(nombre, huffman);
            Aspirante aspirante = new Aspirante();
            aspirante = confidencial.Buscar(codigo);
            if (aspirante == null)
            {
                return View("ErrorBuscarDpi");
            }
            aspirante.infoPriv[0] = huffman.Decodificar(aspirante.infoPriv[0], huffman);
            aspirante.infoPriv[1] = huffman.Decodificar(aspirante.infoPriv[1], huffman);
            List<Aspirante> ListaAspirante = new List<Aspirante>();
            ListaAspirante.Add(aspirante);
            return View("EncontradoDPI", ListaAspirante);
        }
        public string convertidorVec(string[] vector)
        {
            string vectorAString = "";
            if (vector.Count() == 0)
            {
                return vectorAString;
            }
            for (int i = 0; i < vector.Length; i++)
            {
                if (vectorAString == "")
                {
                    vectorAString = vectorAString + vector[i];
                }
                else
                {
                    vectorAString = vectorAString + " " + vector[i];
                }
            }
            return vectorAString;
        }
    }
}
