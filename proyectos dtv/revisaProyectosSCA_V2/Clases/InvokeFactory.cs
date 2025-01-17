using System;
using System.Collections.Generic;
using System.Xml;
using System.Text;

namespace revisaProyectosSCA_V2.Clases
{
    public static class InvokeFactory
    {
        public static InvokeActivity CreateInvoke(XmlNode nodo)
        {
            InvokeActivity i = new InvokeActivity();

            try
            {
                i.partnerLink = nodo.Attributes["partnerLink"].InnerText;
            }
            catch (Exception)
            {
            }
            try
            {
                i.portType = nodo.Attributes["portType"].InnerText;
            }
            catch (Exception)
            {
            }
            try
            {
                i.operation = nodo.Attributes["operation"].InnerText;
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
            try
            {
                i.innerName = nodo.Name;
            }
            catch (Exception e)
            {
                string s = e.Message;
            }
            try
            {
                i.condition = nodo.Attributes["condition"].InnerText;
            }
            catch (Exception e)
            {
                string s = e.Message;
            }
            try
            {
                i.faultVariable = nodo.Attributes["faultVariable"].InnerText;
            }
            catch (Exception e)
            {
                string s = e.Message;
            }


            return i;
        }
    }
}
