using System;
using System.Collections.Generic;
using System.Xml;
using System.Text;

namespace revisaProyectosSCA.Clases
{
    public static class VariableFactory
    {
        public static VariableActivity CreateActivity(XmlNode nodo)
        {
            VariableActivity i = new VariableActivity();

            try
            {
                i.type = nodo.Attributes["type"].InnerText;
            }
            catch (Exception)
            {
            }
            try
            {
                i.messageType = nodo.Attributes["messageType"].InnerText;
            }
            catch (Exception)
            {
            }
            try
            {
                i.name = nodo.Attributes["name"].InnerText;
            }
            catch (Exception e)
            {
                string s = e.Message;
            }

            return i;
        }
    }
}
