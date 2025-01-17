using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Xml;

namespace OER_CargaDatos.Helpers
{
    public static class OERHelper
    {
        static OerProd.FlashlineRegistryService ws;
        static OerProd.AuthToken token;

        internal static void crearServicio()
        {
            if (ws == null)
            {
                ws = new OerProd.FlashlineRegistryService();

                token = ws.authTokenCreate(ConfigurationManager.AppSettings["User"], ConfigurationManager.AppSettings["Password"]);
            }
        }

        internal static List<AssetType> getRelationTypes()
        {
            var res = new List<AssetType>();

            var criteria = new OER_CargaDatos.OerProd.RelationshipTypeCriteria();
            var assetL = ws.relationshipTypeQuery(token, criteria);
            if (assetL != null)
            {
                foreach (var item in assetL.ToList())
                {
                    var at = new AssetType() { Id = item.ID, Name = item.name };
                    res.Add(at);
                }
            }

            return res;
        }

        internal static List<AssetType> getAssetsType()
        {
            var res = new List<AssetType>();

            var criteria = new OER_CargaDatos.OerProd.AssetTypeCriteria();
            var assetL = ws.assetTypeQuery(token, criteria);
            if (assetL != null)
            {
                foreach (var item in assetL.Where(m => m.activeStatus == 0))
                {
                    var at = new AssetType() { Id = item.ID, Name = item.name };
                    res.Add(at);
                }
            }

            return res;
        }

        const int max_assets = 100;

        internal static List<Asset> searchAssets(long typeId, int regStatusId)
        {
            var res = new List<Asset>();

            var criteria = new OER_CargaDatos.OerProd.AssetCriteria();
            criteria.assetTypeCriteria = typeId;

            var assetCount = ws.assetQueryCount(token, criteria);
            OerProd.Asset[] assetL = new OerProd.Asset[0];

            if (assetCount > max_assets)
            {
                var assetSumm = ws.assetQuerySummary(token, criteria);

                var lote = 0;
                do
                {
                    var assetCount2 = ws.assetReadArrayFromSummary(token, assetSumm.Take(max_assets).ToArray());
                    assetL = assetL.Concat(assetCount2).ToArray();
                    assetSumm = assetSumm.Skip(max_assets).ToArray();

                    lote++;

                } while (lote * max_assets < assetCount);
            }
            else
            {
                assetL = ws.assetQuery(token, criteria);
            }

            if (assetL != null)
            {
                foreach (var item in assetL.Where(m => m.registrationStatus == regStatusId || regStatusId == 0))
                {
                    var at = new Asset() { Id = item.ID, Name = item.name, NameFull = string.Format("{0} ({1})", item.name, item.version) };
                    res.Add(at);
                }
            }

            return res;
        }


        internal static void submitAsset(long assetId)
        {
            ws.assetSubmit(token, assetId);
        }
        internal static void acceptAsset(long assetId)
        {
            ws.assetAccept(token, assetId);
        }
        internal static void registerAsset(long assetId)
        {
            ws.assetRegister(token, assetId);
        }

        internal static List<AssetRegistrationStatus> getAssetsRegistrationStatus()
        {
            var res = new List<AssetRegistrationStatus>();
            res.Add(new AssetRegistrationStatus() { Id = 10, Name = "Unsubmitted" });
            res.Add(new AssetRegistrationStatus() { Id = 51, Name = "Submitted" });
            res.Add(new AssetRegistrationStatus() { Id = 52, Name = "Accepted" });
            res.Add(new AssetRegistrationStatus() { Id = 100, Name = "Registered" });

            return res;
        }


        internal static List<Asset> getServicios()
        {
            var res = new List<Asset>();

            var criteria = new OER_CargaDatos.OerProd.AssetCriteria();
            criteria.assetTypeCriteria = 154; //Service = 154
            var assetL = ws.assetQuerySummary(token, criteria);
            if (assetL != null)
            {
                foreach (var item in assetL)
                {
                    var at = new Asset() { Id = item.ID, Name = item.name, TypeId = item.typeID };
                    res.Add(at);
                }
            }

            return res;
        }

        internal static void creaAssetContenido(string assetName, long typeId, long servId, string version, string customDataType, string customDataString)
        {
            var criteria = new OER_CargaDatos.OerProd.AssetCriteria();
            criteria.nameCriteria = assetName;
            criteria.versionCriteria = version;
            var assetSumm = ws.assetQuerySummary(token, criteria);
            if (assetSumm.Count() == 0)
            {
                var assetNew = ws.assetCreate(token, assetName, version, typeId);

                if (servId > 0)
                {
                    assetNew.relationshipTypes[1] = new OerProd.RelationshipType();
                    assetNew.relationshipTypes[1].ID = 108; //Contains
                    assetNew.relationshipTypes[1].primaryIDs = new long[1];
                    assetNew.relationshipTypes[1].primaryIDs[0] = servId;

                    var assetU = ws.assetUpdate(token, assetNew);
                }

                if (customDataType != "" && customDataString != "")
                {
                    var assetU2 = ws.assetUpdateCustomDataString(token, assetNew.ID, customDataType, customDataString);
                }
            }
        }

        internal static List<Asset> getAssetsRelated(long assetId, int relationId, out string customData)
        {
            var res = new List<Asset>();
            customData = "";

            var asset = LeeAssetFull(token, assetId);
            // var asset = ws.assetRead(token, assetId);
            if (asset != null)
            {
                customData = asset.customData;
                foreach (var rel in asset.relationshipTypes)
                {
                    if (rel.ID == relationId)
                    {
                        foreach (var item in rel.secondaryIDs)
                        {
                            res.Add(new Asset() { Id = item });
                        }
                    }
                }
                if (res.Count() > 0)
                {
                    var asSum = new OerProd.AssetSummary[res.Count()];
                    var i = 0;
                    foreach (var item in res)
                    {
                        asSum[i] = new OerProd.AssetSummary() { ID = item.Id };
                        i++;
                    }
                    res = new List<Asset>();
                    var assetL = ws.assetReadArrayFromSummary(token, asSum);
                    foreach (var item in assetL)
                    {
                        res.Add(new Asset() { Id = item.ID, Name = item.name, TypeId = item.typeID });
                    }
                }
            }

            return res;
        }

        internal static void updateCustomData(long assetId, string cdType, string cdValue)
        {
            var asset = ws.assetRead(token, assetId);
            if (asset != null)
            {
                var assetU2 = ws.assetUpdateCustomDataString(token, asset.ID, cdType, cdValue);
            }
        }

        internal static bool creaAssetsRelated(long assetId, long relationType, long[] assetRelatedId)
        {
            var res = false;

            var assetFull = LeeAssetFull(token, assetId);
            if (assetFull != null)
            {
                bool update = false;
                foreach (var rel in assetFull.relationshipTypes)
                {
                    if (rel.ID == relationType)
                    {
                        foreach (var item in assetRelatedId)
                        {
                            if (!rel.primaryIDs.Contains(item))
                            {
                                var ultimo = rel.primaryIDs.Length;
                                var paso = new long[rel.primaryIDs.Length + 1];
                                Array.Copy(rel.primaryIDs, paso, rel.primaryIDs.Length);

                                paso[ultimo] = item;
                                rel.primaryIDs = paso;

                                update = true;
                            }
                        }
                    }
                }

                if (update)
                {
                    var assetU = ws.assetUpdate(token, assetFull);
                    res = true;
                }
            }

            return res;
        }

        internal static bool borraAssetsRelated(long assetId, long relationType, long[] assetRelatedId)
        {
            var res = false;

            var assetFull = LeeAssetFull(token, assetId);
            if (assetFull != null)
            {
                bool update = false;
                foreach (var rel in assetFull.relationshipTypes)
                {
                    if (rel.ID == relationType)
                    {
                        foreach (var item in assetRelatedId)
                        {
                            if (rel.primaryIDs.Contains(item))
                            {
                                rel.primaryIDs = rel.primaryIDs.Where(m => m != item).ToArray();

                                update = true;
                            }
                        }
                    }
                }
                if (update)
                {
                    var assetU = ws.assetUpdate(token, assetFull);
                    res = true;
                }

            }

            return res;
        }

        private static OerProd.Asset LeeAssetFull(OerProd.AuthToken token, long assetId)
        {
            var asset = ws.assetRead(token, assetId);
            if (asset != null)
            {
                var assetXML = ws.assetReadXml(token, assetId);
                if (assetXML != "")
                {
                    var doc = new XmlDocument();
                    doc.LoadXml(assetXML);

                    XmlNodeList bdRel = doc.GetElementsByTagName("relationships");
                    foreach (XmlNode nodeP in bdRel)
                    {
                        XmlElement subsElement = (XmlElement)nodeP;
                        XmlNodeList stepsRel = subsElement.ChildNodes;

                        foreach (XmlNode nodeStepRel in stepsRel)
                        {
                            long relTypeId = long.Parse(nodeStepRel.Attributes["id"].Value);

                            foreach (var rel in asset.relationshipTypes)
                            {
                                if (rel.ID == relTypeId)
                                {
                                    List<long> lPrim = new List<long>();
                                    List<long> lSecn = new List<long>();

                                    XmlNodeList stepsSEP = nodeStepRel.ChildNodes;
                                    foreach (XmlNode nodeP1 in stepsSEP)
                                    {
                                        XmlElement subsElement1 = (XmlElement)nodeP1;

                                        if (nodeP1.Name == "primary")
                                        {
                                            XmlNodeList stepsRelP = nodeP1.ChildNodes;
                                            foreach (XmlNode nodeRel in stepsRelP)
                                            {
                                                long assetRelId = long.Parse(nodeRel.Attributes["id"].Value);
                                                lPrim.Add(assetRelId);
                                            }
                                        }
                                        if (nodeP1.Name == "secondary")
                                        {
                                            XmlNodeList stepsRelP = nodeP1.ChildNodes;
                                            foreach (XmlNode nodeRel in stepsRelP)
                                            {
                                                long assetRelId = long.Parse(nodeRel.Attributes["id"].Value);
                                                lSecn.Add(assetRelId);
                                            }
                                        }
                                    }

                                    rel.primaryIDs = lPrim.ToArray();
                                    rel.secondaryIDs = lSecn.ToArray();
                                }
                            }
                        }
                    }
                }
            }

            return asset;
        }

        internal static object getAplicaciones()
        {
            var res = new List<Asset>();

            var criteria = new OER_CargaDatos.OerProd.AssetCriteria();
            criteria.assetTypeCriteria = 158; //Service = 158
            var assetL = ws.assetQuerySummary(token, criteria);
            if (assetL != null)
            {
                foreach (var item in assetL)
                {
                    var at = new Asset() { Id = item.ID, Name = item.name, TypeId = item.typeID };
                    res.Add(at);
                }
            }

            return res;
        }

        internal static string LeeAssetFullXML(long assetId)
        {
            var assetXML = ws.assetReadXml(token, assetId);
            if (assetXML != "") { return assetXML; }
            else { return ""; }
        }

        internal static OerProd.Asset LeeAssetName(long assetId)
        {
            return ws.assetRead(token, assetId);
        }
    }
}
