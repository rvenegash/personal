using System;
using System.Collections.Generic;
using System.Xml;
using System.Text;

namespace revisaProyectosSCA.Clases
{
    public static class GenericFactory
    {
        public static GenericActivity CreateGenericActivity(XmlNode nodo)
        {
            GenericActivity i = new GenericActivity();

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

            try
            {
                i.target = nodo.Attributes["target"].InnerText;
            }
            catch (Exception e)
            {
                string s = e.Message;
            }
            try
            {
                i.variable = nodo.Attributes["variable"].InnerText;
            }
            catch (Exception e)
            {
                string s = e.Message;
            }
            try
            {
                i.expression = nodo.Attributes["expression"].InnerText;
            }
            catch (Exception e)
            {
                string s = e.Message;
            }
            try
            {
                i.bpelx_inputHeaderVariable = nodo.Attributes["bpelx:inputHeaderVariable"].InnerText;
            }
            catch (Exception e)
            {
                string s = e.Message;
            }
            try
            {
                i.inputVariable = nodo.Attributes["inputVariable"].InnerText;
            }
            catch (Exception e)
            {
                string s = e.Message;
            }
            try
            {
                i.outputVariable = nodo.Attributes["outputVariable"].InnerText;
            }
            catch (Exception e)
            {
                string s = e.Message;
            }
            try
            {
                i.indexVariable = nodo.Attributes["indexVariable"].InnerText;
            }
            catch (Exception e)
            {
                string s = e.Message;
            }


            return i;
        }
    }
}
