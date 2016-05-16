using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using Microsoft.Xna.Framework.Storage;

namespace The_Dream.Classes
{
    public class XmlManager<T>
    {
        public Type type;
        public XmlManager()
        {
            type = typeof(T);
        }
        public T Load(string path)
        {
            T instance;
            using (TextReader reader = new StreamReader(path))
            {
                XmlSerializer xml = new XmlSerializer(type);
                instance = (T)xml.Deserialize(reader);
            }
            return instance;
        }
        public void Save()
        {

        }
    }
}