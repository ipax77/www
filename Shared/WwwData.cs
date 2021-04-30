using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace www.pwa.Shared
{
    public class WwwData
    {
        public static string[] s_classes = new string[] {
            "0(Lehrer)",
            "5a", "5b", "5c", "5d", "5e",
            "6a", "6b", "6c", "6d", "6e", "6f",
            "7a", "7b", "7c", "7d", "7e",
            "8a", "8b+", "8c+", "8d+",
            "9a", "9b+", "9c+", "9d+", "9e+", "9b++", "9c++", "9d++", "9e++",
            "10a", "10b+", "10c+", "10d+",
            "11",
            "12"
        };
        
        public static string s_school = "Gymnasium Geretsried";
        public static string s_walk = "Weltumrundung";
        public static float TotalDistance = 40075f;
        public static string Credential = "test123";
        public static List<WwwPathPoint> pathPoints = new List<WwwPathPoint>();
        public static string Admin = String.Empty;
        public static string AdminCredential = String.Empty;

        public static Dictionary<string, float> Points = new Dictionary<string, float>()
        {
            { "Prinz-Georg-Land - Земля Георга, Russland", 0 }, // https://de.wikipedia.org/wiki/Prinz-Georg-Land
            { "Sewerny-Insel - Северный остров, Russland",  10}, // https://de.wikipedia.org/wiki/Sewerny-Insel
            { "Varanger",  23}, //
            { "Kemi, Finnland", 47 }, // https://de.wikipedia.org/wiki/Kemi
            { "Mariehamn - Maarianhamina, Åland",  51}, // https://de.wikipedia.org/wiki/Mariehamn
            { "Malmö, Schweden",  62.5f},
            { "Rostock, Deutschland", 75 },
            { "Leipzig", 80 },
            { "Geretsried", 90 }
        };

        public static List<WwwInterest> Interests = new List<WwwInterest>()
        {
            new WwwInterest() { Name = "Polen", Distance = 0, Percentage = new float[3] { 4, 0, 0 } },
            new WwwInterest() { Name = "Weißrussland (Belarus)", Distance = 0, Percentage = new float[3] { 9, 0, 0 } },
            new WwwInterest() { Name = "Russland", Distance = 0, Percentage = new float[3] { 14, 0, 0 } },
            new WwwInterest() { Name = "Moskau (Москва́)", Distance = 0, Percentage = new float[3] { 19, 0, 0 } },
            new WwwInterest() { Name = "Nischni Nowgorod (Нижний Новгород)", Distance = 0, Percentage = new float[3] { 22, 0, 0 } },
            new WwwInterest() { Name = "Uralgebirge (Уральские горы)", Distance = 0, Percentage = new float[3] { 33, 0, 0 } },
            new WwwInterest() { Name = "Fluss Jenissei (Енисей)", Distance = 0, Percentage = new float[3] { 42, 0, 0 } },
            new WwwInterest() { Name = "Landschaftsschutzgebiet Putoranskiy Gosudarstvennyy Prirodnyy Zapovednik", Distance = 0, Percentage = new float[3] { 50, 0, 0 } },
            new WwwInterest() { Name = "Tschuktschensee (Чуко́тское мо́ре)", Distance = 0, Percentage = new float[3] { 90, 13.5f, 0 } },
            new WwwInterest() { Name = "Alaska", Distance = 0, Percentage = new float[3] { 0, 15, 0 } },
            new WwwInterest() { Name = "Kanada", Distance = 0, Percentage = new float[3] { 0, 21.8f, 0 } },
            new WwwInterest() { Name = "Großer Bärensee (Grand lac de l’Ours)", Distance = 0, Percentage = new float[3] { 0, 26.5f, 0 } },
            new WwwInterest() { Name = "Naturschutzgebiet Queen Maud Gulf Bird Sanctuary", Distance = 0, Percentage = new float[3] { 0, 36, 0 } },
            new WwwInterest() { Name = "Repulse Bay (Naujaat - \"Ort, wo Möwen nisten\")", Distance = 0, Percentage = new float[3] { 0, 44, 0 } },
            new WwwInterest() { Name = "Qikiqtarjuaq (\"Große Insel\")", Distance = 0, Percentage = new float[3] { 0, 63, 23 } },
            new WwwInterest() { Name = "Grönland (Kalaallit Nunaat)", Distance = 0, Percentage = new float[3] { 0, 69, 27 } },
            new WwwInterest() { Name = "Island", Distance = 0, Percentage = new float[3] { 0, 84, 41 } },
            new WwwInterest() { Name = "Schottland (Scotland)", Distance = 0, Percentage = new float[3] { 0, 93, 62 } },
            new WwwInterest() { Name = "London", Distance = 0, Percentage = new float[3] { 0, 0, 76 } },
            new WwwInterest() { Name = "Frankreich (France)", Distance = 0, Percentage = new float[3] { 0, 0, 81 } },
            new WwwInterest() { Name = "Orléans", Distance = 0, Percentage = new float[3] { 0, 0, 86 } },
            new WwwInterest() { Name = "Deutschland", Distance = 0, Percentage = new float[3] { 0, 0, 95 } },
            new WwwInterest() { Name = "München", Distance = 0, Percentage = new float[3] { 0, 0, 99 } },
            new WwwInterest() { Name = "Geretsried", Distance = 0, Percentage = new float[3] { 0, 0, 100 } },
        };
    }
}
