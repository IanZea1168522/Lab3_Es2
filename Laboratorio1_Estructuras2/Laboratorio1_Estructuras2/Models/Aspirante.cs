﻿namespace Laboratorio1_Estructuras2.Models
{
    public class Aspirante : IComparable<Aspirante>
    {
        public string nombre { get; set; }
        public List<string> infoPriv { get; set; }
        public string nacimiento { get; set; }
        public string direccion { get; set; }
        public List<string> carta { get; set; } 
        public List<Dictionary<string, int>> diccionario { get; set; }
        public int CompareTo(Aspirante other)
        {
            int result = this.infoPriv[0].CompareTo(other.infoPriv[0]);
            return result;
        }
    }
}
