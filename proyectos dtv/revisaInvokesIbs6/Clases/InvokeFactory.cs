using System;
using System.Collections.Generic;
using System.Xml;
using System.Text;

namespace revisaInvokesIbs6.Clases
{
    public static class InvokeFactory
    {
        public static Invoke CreateInvoke(XmlNode nodo)
        {
            Invoke i = new Invoke();

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
