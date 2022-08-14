using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace huarongdao
{
    class Program
    {
        static void Main1(string[] args)
        {
            PizzeResolver resolver = new PizzeResolver();
            resolver.bounds = new List<BoundsInt>() {
                new BoundsInt(0, 0, 0, 10, 10, 1)
            };
            resolver.titles = new List<Title>() {
                new Title() {
                    id = 0,
                    elements = new List<Vector3Int>() {
                        new Vector3Int(1,1,(int)PizzeResolver.Direct.u)
                    }
                },
                new Title()
                {
                    id = 1,
                    elements = new List<Vector3Int>() {
                        new Vector3Int(1,5,(int)PizzeResolver.Direct.r)
                    }
                },
                new Title()
                {
                    id = 2,
                    elements = new List<Vector3Int>() {
                        new Vector3Int(5,4,(int)PizzeResolver.Direct.l)
                    }
                },
                new Title()
                {
                    id = 3,
                    elements = new List<Vector3Int>() {
                        new Vector3Int(7,5,(int)PizzeResolver.Direct.d)
                    }
                },
                new Title()
                {
                    id = 4,
                    elements = new List<Vector3Int>() {
                        new Vector3Int(7,0,(int)PizzeResolver.Direct.l)
                    }
                }
            };
            //resolver.nTitles.Add(0);
            //resolver.nTitles.Add(1);
            //resolver.nTitles.Add(2);
            //resolver.nTitles.Add(3);
            resolver.nTitles.Add(4);


            Log.Print(resolver.ToString());
            Log.Print(resolver.Move(0, (byte)PizzeResolver.Direct.u));
            Log.Print(resolver.ToString());
            Log.Print(resolver.Move(2, (byte)PizzeResolver.Direct.l));
            Log.Print(resolver.ToString());
            Log.Print(resolver.Move(1, (byte)PizzeResolver.Direct.r));
            Log.Print(resolver.ToString());
            Log.Print(resolver.Move(3, (byte)PizzeResolver.Direct.d));
            Log.Print(resolver.ToString());
            Log.Print(resolver.Move(4, (byte)PizzeResolver.Direct.l));
            Log.Print(resolver.ToString());

            Console.ReadKey();
        }
        static void Main(string[] args)
        {
            int iteration = -1;
            PizzeResolver pizze = new PizzeResolver();
            pizze.bounds = new List<BoundsInt>() {
                new BoundsInt(0, 0, 0, 8, 8, 1)
            };
            pizze.random = new System.Random(123456);

            PizzeGenerate generate = new PizzeGenerate();
            generate.pizze = pizze;
            generate.jh_max = 0.1f;
            generate.null_max = 0f;
            generate.iteration = 1;

            do
            {
                int seed = DateTime.Now.Millisecond;
                int titCount = 0;
                for (int i = 0; i <= int.MaxValue; i++)
                {
                    pizze.random = new System.Random(seed);
                    generate.iteration = i;
                    generate.GeneratePizze();

                    //Log.Print("[seed," + seed + "][iteration," + i + "]" + pizze.titles.Count + " " + pizze.nTitles.Count + " " + pizze.dTitles.Count + "\n" + pizze.ToString());

                    if (titCount != pizze.titles.Count) titCount = pizze.titles.Count;
                    else break;
                }
                Log.Print("[seed," + seed + "][iteration," + generate.iteration + "]" + pizze.titles.Count + " " + pizze.nTitles.Count + " " + pizze.dTitles.Count + "\n" + pizze.ToString());

                PizzeParse parse = new PizzeParse() { 
                    pizze = pizze
                };

                bool par = parse.Parse(); 
                Log.Print("Parse:"+ par + "[maxStep,"+ parse.maxStep+ "][outLoop," + parse.outLoop + "][seed, " + seed);
                Log.Print("[seed," + seed + "]" + pizze.titles.Count + " " + pizze.nTitles.Count + " " + pizze.dTitles.Count + "\n" + parse.pizze.ToString());

                if (!par) break;

            } while (Console.ReadKey().Key == ConsoleKey.Spacebar && Console.ReadKey().Key != ConsoleKey.Escape);

            while (Console.ReadKey().Key != ConsoleKey.Escape)
            { }
        }
    }
    public static class Log{
        public static void Print(object ms)
        {
            Console.WriteLine(ms);
        }
        public static void PrintAppend(object ms)
        {
            Console.Write(ms);
        }
    }

    public class Title {

        //public int parent= -1;
        public int id;
        public List<Vector3Int> elements;

        public bool Contion(int x,int y) {

            for (int i = 0; i < elements.Count; i++)
            {
                if (elements[i].x == x && elements[i].y == y) return true;
            }
            return false;
        }
        public int Index(int x, int y)
        {
            for (int i = 0; i < elements.Count; i++)
            {
                if (elements[i].x == x && elements[i].y == y) return i;
            }
            return -1;
        }

        public override string ToString()
        {
            if (elements != null && elements.Count != 0) return elements[0].ToString();
            return id.ToString();
        }

        public Title Clone() {
            Title title = new Title();
            title.id = id;
            title.elements = new List<Vector3Int>(elements);
            return title;
        }
    }

    public class HrdResolver : PizzeResolver
    {


    }

    public class PizzeResolver {

        public System.Random random = new System.Random(123456);
        public float radFloat { get { return (float)random.NextDouble(); } }

        public enum Direct : byte
        { 
            u = 1,       //0000 0001
            r = 2,       //0000 0010
            d = 4,       //0000 0100
            l = 8,       //0000 1000
            ur = 16,     //0001 0000
            dr = 32,     //0010 0000
            dl = 64,     //0100 0000
            ul = 128,    //1000 0000
        }
        public byte[] directBytes = new byte[] {
            1,       //0000 0001
            2,       //0000 0010
            4,       //0000 0100
            8,       //0000 1000
            16,     //0001 0000
            32,     //0010 0000
            64,     //0100 0000
            128,    //1000 0000
        };
        public Dictionary<byte, Vector3Int> directs = new Dictionary<byte, Vector3Int>() {
            { 1,  new Vector3Int(0,1,0)},
            { 2,  new Vector3Int(1,0,0)},
            { 4,  new Vector3Int(0,-1,0)},
            { 8,  new Vector3Int(-1,0,0)},
            { 16, new Vector3Int(1,1,0)},
            { 32, new Vector3Int(1,-1,0)},
            { 64, new Vector3Int(-1,-1,0)},
            { 128,new Vector3Int(-1,1,0)},
        };
        public Dictionary<int, string> directStr = new Dictionary<int, string>() {
            { 1,  "↑"},
            { 2,  "→"},
            { 4,  "↓"},
            { 8,  "←"},
            { 16, "↗"},
            { 32, "↘"},
            { 64, "↙"},
            { 128,"↖"},
        };

        public List<BoundsInt> bounds = new List<BoundsInt>();
        public List<Title> titles = new List<Title>();

        public List<int> nTitles = new List<int>();
        public List<int> dTitles = new List<int>();

        public virtual PizzeResolver Clone() {
            PizzeResolver value = new PizzeResolver();
            value.random = random;
            value.bounds = new List<BoundsInt>(bounds);
            value.nTitles = new List<int>(nTitles);
            value.dTitles = new List<int>(dTitles);

            value.titles = new List<Title>();
            for (int i = 0; i < titles.Count; i++)
            {
                value.titles.Add(titles[i].Clone());
            }
            return value;
        }

        public virtual bool Move(int titleID, byte dirCode)
        {
            if(!nTitles.Contains(titleID)) { Log.Print("not in nTitle"); return false; }

            if (!InBound(titleID)) { Log.Print("not in bound"); return false; }

            if (!CanMove(titleID, dirCode)) { Log.Print("not move"); return false; }

            return MoveLengthForDirect(titleID, dirCode) != 0;
        }
        public Vector2Int GetPos(int x, int y,List<int> outs =null) {
            for (int i = 0; i < nTitles.Count; i++)
            {
                int id = nTitles[i];
                if (outs != null && outs.Contains(id)) continue;

                int posID = titles[id].Index(x, y);
                if (posID > -1) return new Vector2Int(id,posID);
            }
            return new Vector2Int(-1, -1);
        }
        public int MoveLengthForDirect(int titleID, byte dirCode,bool autoMove = true)
        {
            Vector3Int direct = GetDirection(dirCode);
            var title = titles[titleID];

            int fCount = 0;
            int min = int.MaxValue;
            int fmin = int.MaxValue;

            for (int n = 0; n < title.elements.Count; n++)
            {
                int len = MoveLengthForDirect(titleID, n, direct);

                if (len < 0) fCount++;
                if (len > -1 && len < min) min = len;
                if (len < 0 && len < fmin) fmin = len;
            }

            if (min == int.MaxValue)
            {
                if (autoMove)
                {
                    nTitles.Remove(titleID);
                    dTitles.Add(titleID);

                    Vector3Int path = direct * (-fmin);
                    for (int i = 0; i < title.elements.Count; i++)
                    {
                        title.elements[i] += path;
                    }
                }

                return -1;
            }
            else {

                if (min == 0)
                {
                    return 0;
                }
                else
                {
                    if (autoMove)
                    {
                        Vector3Int path = direct * min;
                        for (int i = 0; i < title.elements.Count; i++)
                        {
                            title.elements[i] += path;
                        }
                    }
                    return min;
                }
            }

        }

        protected int MoveLengthForDirect(int titleID,int eleID, Vector3Int direct)
        {
            Vector3Int elePos = titles[titleID].elements[eleID];
            elePos.z = 0;

            Vector2Int p = new Vector2Int(elePos.x+ direct.x, elePos.y+ direct.y);
            bool canMove = true;
            int len = -1;
            while (InBound(p.x, p.y))
            {
                canMove = true;

                foreach (var id in nTitles)
                {
                    if (id == titleID) continue;

                    if (titles[id].Contion(p.x, p.y)) { canMove = false; break; }
                }
                if (canMove)
                {
                    len = len == -1 ? 1 : len + 1;

                    p.x += direct.x;
                    p.y += direct.y;
                }
                else {

                    len = len == -1 ? 0 : len;
                    break;
                }
            }
            len = canMove ? -Mathf.Abs(len) : len;

            return len;
        }
        
        public int MoveLength(int x,int y, Vector3Int direct)
        {
            Vector2Int p = new Vector2Int(x + direct.x, y + direct.y);
            bool canMove = true;
            int len = -1;
            while (InBound(p.x, p.y))
            {
                canMove = true;

                foreach (var id in nTitles)
                {
                    if (titles[id].Contion(p.x, p.y)) { canMove = false; break; }
                }
                if (canMove)
                {
                    len = len == -1 ? 1 : len + 1;

                    p.x += direct.x;
                    p.y += direct.y;
                }
                else
                {

                    len = len == -1 ? 0 : len;
                    break;
                }
            }
            len = canMove ? -Mathf.Abs(len) : len;

            return len;
        }

        protected Vector3Int GetDirection(byte dirCode) {
            return directs[dirCode];
        }

        protected bool CanMove(int titleID, byte dirCode) {

            return titles[titleID].elements.All(x => ((byte)x.z & dirCode) > 0);
        }

        public bool InBound(int titleID) {
            var title = titles[titleID];
            for (int i = 0; i < bounds.Count; i++)
            {
                for (int n = 0; n < title.elements.Count; n++)
                {
                    Vector3Int pos = title.elements[n];
                    pos.z = 0;

                    if (bounds[i].Contains(pos)) {
                        return true;
                    }
                }
            }
            return false;
        }
        public bool InBound(Title title)
        {
            for (int i = 0; i < bounds.Count; i++)
            {
                for (int n = 0; n < title.elements.Count; n++)
                {
                    Vector3Int pos = title.elements[n];
                    pos.z = 0;

                    if (bounds[i].Contains(pos))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        public bool InBound(int x,int y)
        {
            Vector3Int p = new Vector3Int(x, y, 0);
            for (int i = 0; i < bounds.Count; i++)
            {
                if (bounds[i].Contains(p))
                {
                    return true;
                }
            }
            return false;
        }

        protected List<int> NearTitles(int titleID)
        {
            List<int> nears = new List<int>();
            List<int> ths = new List<int>() { titleID };

            var title = titles[titleID];
            for (int i = 0; i < bounds.Count; i++)
            {
                for (int n = 0; n < title.elements.Count; n++)
                {
                    Vector3Int pos = title.elements[n];
                    pos.z = 0;

                    for (int d = 0; d < 8; d++)
                    {
                        Vector3Int p = pos + directs[directBytes[d]];
                        Vector2Int pID = GetPos(p.x, p.y, ths);

                        if (!nears.Contains(pID.x)) {
                            nears.Add(pID.x);
                        }
                    }
                }
            }
            return nears;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            Vector2Int min = new Vector2Int(int.MaxValue, int.MaxValue);
            Vector2Int max = new Vector2Int(int.MinValue, int.MinValue);

            foreach (var item in bounds)
            {
                min.x = Mathf.Min(item.xMin, min.x);
                min.y = Mathf.Min(item.yMin, min.y);
                max.x = Mathf.Max(item.xMax, max.x);
                max.y = Mathf.Max(item.yMax, max.y);
            }

            Vector2Int off = min * -1;

            Dictionary<Vector2Int, string> str = new Dictionary<Vector2Int, string>();
            for (int y = min.y; y < max.y; y++)
            {
                for (int x = min.x; x < max.x; x++)
                {
                    int _x = x + off.x;
                    int _y = y + off.y;

                    if (InBound(x, y))
                    {
                        str.Add(new Vector2Int(_x, _y), "□");
                    }
                    else {

                        str.Add(new Vector2Int(_x, _y), "■");
                    }
                }
            }
            foreach (var n in nTitles)
            {
                foreach (var item in titles[n].elements)
                {
                    int _x = item.x + off.x;
                    int _y = item.y + off.y;
                    str[new Vector2Int(_x, _y)] = directStr[item.z];
                }
            }
            for (int y = max.y - 1; y >= min.y; y--)
            {
                for (int x = min.x; x < max.x; x++)
                {
                    int _x = x + off.x;
                    int _y = y + off.y;

                    sb.Append(str[new Vector2Int(_x, _y)]);
                }
                sb.Append("\r\n");
            }
            return sb.ToString();
        }
    }

    public class PizzeGenerate
    {
        public PizzeResolver pizze;

        public float jh_max = 0.1f;
        public float null_max = 0;

        public int iteration = 1;

        int[] radArray = new int[4] { 0, 1, 2, 3 };

        public Dictionary<byte, byte> directF = new Dictionary<byte, byte>() {
            { 1,  4},
            { 2,  8},
            { 4,  1},
            { 8,  2},
            { 16, 64},
            { 32, 128},
            { 64, 16},
            { 128,32},
        };
        int[] RadArray()
        {
            for (int i = 0; i < radArray.Length; i++)
            {
                radArray[i] = i;
            }
            for (int i = 0; i < radArray.Length; i++)
            {
                int r = pizze.random.Next(0, 4);
                int t = radArray[i];
                radArray[i] = radArray[r];
                radArray[r] = t;
            }
            return radArray;
        }

        public void GeneratePizze()
        {
            pizze.titles.Clear();
            pizze.nTitles.Clear();
            pizze.dTitles.Clear();

            List<Vector2Int> points = new List<Vector2Int>();
            foreach (var item in pizze.bounds)
            {
                for (int y = item.min.y; y < item.max.y; y++)
                {
                    for (int x = item.min.x; x < item.max.x; x++)
                    {
                        Vector2Int p = new Vector2Int(x, y);

                        if (!points.Contains(p)) points.Add(p);
                    }
                }
            }
            int jhCount = Mathf.CeilToInt(jh_max * pizze.radFloat * points.Count);

            for (int i = 0; i < jhCount; i++)
            {
                int index = Mathf.FloorToInt(pizze.radFloat * points.Count);
                Vector2Int p = points[index];
                points.RemoveAt(index);

                int dircode = -1;

                RadArray();
                for (int r = 0; r < radArray.Length; r++)
                {
                    byte dirCode = (byte)(1 << radArray[r]);
                    if (pizze.MoveLength(p.x, p.y, pizze.directs[dirCode]) < 0)
                    {
                        dircode = dirCode;
                        break;
                    }
                }

                if (dircode > -1)
                {
                    Title title = new Title()
                    {
                        id = pizze.titles.Count,
                        elements = new List<Vector3Int>() {
                            new Vector3Int(p.x,p.y,(byte)dircode)
                        }
                    };

                    pizze.titles.Add(title);
                    pizze.nTitles.Add(title.id);
                }
                else
                {
                    Log.Print("Error direct == -1:" + p);
                }
            }

            List<List<int>> derList = new List<List<int>>(jhCount);

            //TODO--------------------
            for (int j = 0; j < pizze.nTitles.Count; j++)
            {
                derList.Add(new List<int>());
                derList[j] = new List<int>() {
                    pizze.nTitles[j]
                };
            }
            for (int i = 0; i < iteration; i++)
            {
                for (int j = 0; j < derList.Count; j++)
                {
                    List<int> adds = new List<int>();
                    for (int n = 0; n < derList[j].Count; n++)
                    {
                        var list = Derivative(derList[j][n]);
                        for (int k = 0; k < list.Count; k++)
                        {
                            adds.Add(list[k]);
                        }
                    }
                    derList[j] = adds;
                }
            }

            int nullCount = Mathf.CeilToInt(null_max * pizze.radFloat * points.Count);
            for (int i = 0; i < nullCount; i++)
            {
                if (pizze.nTitles.Count > 0) {

                    int index = Mathf.FloorToInt(pizze.radFloat * pizze.nTitles.Count);
                    int id = pizze.nTitles[index];
                    pizze.nTitles.RemoveAt(index);
                    pizze.dTitles.Add(id);
                }
            }
        }
        private List<byte> GetCanMoveDirs(int x, int y) {

            List<byte> list = new List<byte>();
            for (int i = 0; i < 4; i++)
            {
                byte dir = pizze.directBytes[radArray[i]];

                int len = pizze.MoveLength(x,y, pizze.directs[dir]);
                if (len != 0) {
                    list.Add(dir);
                }
            }
            return list;
        }

        private bool HavF(int x, int y, byte dirCode) {

            Vector3Int dir = pizze.directs[dirCode];
            Vector3Int p = new Vector3Int(x, y, 0) + dir;

            while (pizze.InBound(p.x,p.y))
            {
                Vector2Int t = pizze.GetPos(p.x, p.y);
                if (t.x > -1) {
                    if (pizze.titles[t.x].elements[0].z == directF[dirCode]) {
                        return true;
                    }
                }
                p += dir;
            }
            return false;
        }

        private bool IsLoop(int x,int y,Vector3Int dir) {

            Vector3Int p = new Vector3Int(x, y, 0);
            Vector3Int s = p;

            while (pizze.InBound(p.x, p.y))
            {
                Vector2Int t = pizze.GetPos(p.x, p.y);
                if (t.x > -1)
                {
                    p = pizze.titles[t.x].elements[0];
                    dir = pizze.directs[(byte)p.z];
                }
                p += dir;

                if (p.x == s.x && p.y == s.y) {
                    return true;
                }
            }
            return false;
        }

        private List<int> Derivative(int titleID)
        {
            List<int> result = new List<int>();
            var title = pizze.titles[titleID];
            Vector3Int p = title.elements[0];

            byte dir = (byte)title.elements[0].z;

            RadArray();
            for (int i = 0; i < 4; i++)
            {
                byte dir1 = pizze.directBytes[radArray[i]];
                Vector3Int d = p + pizze.directs[dir1];

                if (!pizze.InBound(d.x, d.y)) continue;
                if (pizze.GetPos(d.x, d.y).x > -1) continue;

                int c = Mathf.FloorToInt(pizze.radFloat * 4);
                for (int j = 0; j < 4; j++)
                {
                    int n = (j + c) % 4;

                    byte dir2 = pizze.directBytes[radArray[n]];
                    d.z = dir2;
                    Vector3Int d2 = pizze.directs[dir2];

                    //Log.Print(pizze.ToString());
                    bool canAdd = false;
                    if (!(dir2 == directF[dir] && dir2 == directF[dir1]))       //排除 !↕  !↔
                    {

                        int len = pizze.MoveLength(d.x, d.y, d2);
                        if (len >= 0)
                        {
                            //排除对立
                            if (!HavF(d.x, d.y, dir2))
                            {
                                if (len == 0)
                                {
                                    //排除循环
                                    if (!IsLoop(d.x, d.y, d2))
                                    {
                                        canAdd = true;
                                    }
                                }
                                else canAdd = true;
                            }
                        }
                        else
                        {
                            canAdd = true;
                        }
                    }

                    if (canAdd)
                    {
                        Title tit = new Title()
                        {
                            id = pizze.titles.Count,
                            elements = new List<Vector3Int>() { d }
                        };

                        var res = pizze.titles.FirstOrDefault(x => x.elements[0].x == d.x && x.elements[0].y == d.y);

                        if (res != null) {
                            Log.Print(res);
                        }
                        pizze.titles.Add(tit);
                        pizze.nTitles.Add(tit.id);

                        result.Add(tit.id);
                        break;
                    }
                }
            }

            return result;
        }

    }

    public class PizzeParse {

        public PizzeResolver pizze;
        private Dictionary<byte, byte> directF = new Dictionary<byte, byte>() {
            { 1,  4},
            { 2,  8},
            { 4,  1},
            { 8,  2},
            { 16, 64},
            { 32, 128},
            { 64, 16},
            { 128,32},
        };

        private int m_MaxStep = 0;
        public int maxStep => m_MaxStep;
        public bool Parse() {
            num = 0;
            m_MaxStep = 0;
            //return DieDai() && pizze.nTitles.Count == 0;

            var piz = pizze.Clone();
            pizze = piz.Clone();
            outLoop = false;
            if (DieDai() && pizze.nTitles.Count == 0)
            {
                return true;
            }
            else
            {
                num = 0;
                m_MaxStep = 0;
                outLoop = true; 
                pizze = piz.Clone();
                return DieDai() && pizze.nTitles.Count == 0;
            }
        }

        private bool Move(out Vector3Int p,out int titleID) {

            for (int i = 0; i < pizze.nTitles.Count; i++)
            {
                var title = pizze.titles[pizze.nTitles[i]];
                p = title.elements[0];
                titleID = title.id;
                int len = pizze.MoveLengthForDirect(title.id, (byte)title.elements[0].z);
                if (len != 0)
                {
                    return true;
                }
            }
            p = Vector3Int.zero;
            titleID = -1;
            return false;
        }

        private List<int> CanMoves() {

            List<int> list = new List<int>();
            for (int i = 0; i < pizze.nTitles.Count; i++)
            {
                var title = pizze.titles[pizze.nTitles[i]];
                int len = pizze.MoveLengthForDirect(title.id, (byte)title.elements[0].z,false);
                if (len != 0)
                {
                    list.Add(title.id);
                }
            }
            return list;
        }

        public bool outLoop = false;
        private bool IsLoop(int x, int y, Vector3Int dir)
        {
            Vector3Int p = new Vector3Int(x, y, 0);
            Vector3Int s = p;

            int i = 0;
            while (pizze.InBound(p.x, p.y))
            {
                if (i++ > 100) return false;
                Vector2Int t = pizze.GetPos(p.x, p.y);
                if (t.x > -1)
                {
                    p = pizze.titles[t.x].elements[0];
                    dir = pizze.directs[(byte)p.z];
                }
                p += dir;

                if (p.x == s.x && p.y == s.y)
                {
                    return true;
                }
            }
            return false;
        }

        int num = 0;
        private bool DieDai(int c = 0)
        {
            m_MaxStep = Mathf.Max(m_MaxStep, num);

            if (num++ > pizze.titles.Count * 10) { Log.Print("num == " + num); return true; }

            int[] titles = pizze.nTitles.ToArray();
            for (int i = titles.Length - 1; i >= 0; i--)
            {
                var title = pizze.titles[titles[i]];
                Vector3Int pos = title.elements[0];
                int titleID = title.id;
                int len = pizze.MoveLengthForDirect(title.id, (byte)title.elements[0].z);

                if (len != 0)
                {
                    if (outLoop && len != -1)
                    {
                        //排除循环
                        Vector3Int p = pizze.titles[titleID].elements[0];
                        if (IsLoop(p.x, p.y, pizze.directs[(byte)p.z]))
                        {
                            if (pizze.dTitles.Contains(titleID))
                            {
                                pizze.dTitles.Remove(titleID);
                                pizze.nTitles.Add(titleID);
                            }

                            pizze.titles[titleID].elements[0] = pos;
                            continue;
                        }
                    }

                    //Log.Print(c+"步进["+pizze.titles.Count+","+titles.Length+","+i+"]"+title.id +"\n"+pizze.ToString());
                    if (DieDai(c+1))
                    {
                        return true;
                    }
                    else
                    {
                        if (pizze.dTitles.Contains(titleID)) {

                            pizze.dTitles.Remove(titleID);
                            pizze.nTitles.Add(titleID);
                        }

                        pizze.titles[titleID].elements[0] = pos;
                        //Log.Print(c+ "层集[" + pizze.titles.Count + "," + titles.Length + "," + i + "]" + title.id + "\n" + pizze.ToString());
                    }
                }
                else if (i == 0 && pizze.nTitles.Count != 0)
                {
                    //Log.PrintAppend(c + "退层");
                    return false;
                }
            }
            if (pizze.nTitles.Count != 0)
            {
                //Log.PrintAppend(c + "退层");
                return false;
            }
            else {
                //Log.Print("num == " + num);
                return true;
            }
        }
    }
}
