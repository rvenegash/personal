namespace winVLSJsonExplorer
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void bBuscar_Click(object sender, EventArgs e)
        {
            bCargar.Enabled = false;
            openFileDialog1.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory;

            var opr = openFileDialog1.ShowDialog();

            if (opr == DialogResult.OK)
            {
                if (!File.Exists(openFileDialog1.FileName))
                    return;

                tbArchivo.Text = openFileDialog1.FileName;

                bCargar.Enabled = true;
            }
        }

        private void bCargar_Click(object sender, EventArgs e)
        {
            var sr = new StreamReader(openFileDialog1.FileName);
            var apiText = sr.ReadToEnd();
            var apiObj = System.Text.Json.JsonSerializer.Deserialize<VLSResponseDto>(apiText);

            treeView1.Nodes.Clear();
            var rutinas = new Rutinas();

            if (apiObj.Schedules != null)
            {
                foreach (var schedule in apiObj.Schedules)
                {
                    var channelList = apiObj.Channels.Where(m => m.VlsChannelId == schedule.ChannelId).Select(d => new { d.Css, d.VcNumber, d.ShortName }).Distinct().ToList();

                    TreeNode node = new TreeNode();
                    node.Tag = schedule;
                    node.Text = $"{schedule.ScheduleId}-{schedule.ScheduleDate}-C:{channelList.Count}-E:{schedule.Events.Count}";

                    TreeNode nodeC = new TreeNode("Canales");
                    foreach (var channel in channelList)
                    {
                        var lPaises = rutinas.getPaises(channel.Css);

                        TreeNode nodeC1 = new TreeNode($"{channel.ShortName}-{channel.VcNumber}-{String.Join(".", lPaises.ToArray())}");
                        //nodeC1.Tag = channel;
                        nodeC.Nodes.Add(nodeC1);
                    }
                    node.Nodes.Add(nodeC);

                    TreeNode nodeE = new TreeNode("Eventos");
                    foreach (var eventO in schedule.Events)
                    {
                        var program = apiObj.Programs.FirstOrDefault(m => m.programId == eventO.ProgramId || m.Id == eventO.ProgramId);
                        var nomProgram = program?.Titles?.FirstOrDefault();

                        TreeNode nodeE1 = new TreeNode($"{nomProgram.Value}-{eventO.StartDate}-{eventO.EndDate}");
                        nodeE1.NodeFont = new Font(treeView1.Font, (eventO.PurchaseInfo is null) ? FontStyle.Regular : FontStyle.Bold);
                        //nodeE1.Tag = eventO;
                        if (nodeE1.NodeFont.Bold)
                        {
                            node.NodeFont = new Font(treeView1.Font, (eventO.PurchaseInfo is null) ? FontStyle.Regular : FontStyle.Bold);
                            nodeE1.EnsureVisible();
                        }
                        nodeE.Nodes.Add(nodeE1);
                    }
                    node.Nodes.Add(nodeE);
                    treeView1.Nodes.Add(node);



                    ////if (channelList is null || channelList.Count == 0)
                    ////{
                    ////    _logger.LogInformation($"   no se encontro canal {schedule.ChannelId}");
                    ////    continue;
                    ////}
                    //foreach (var channel in channelList)
                    //{
                    //    var lPaises = rutinas.getPaises(channel.Css);

                    //    var list = new List<EventScheduleInformationRequestType>();

                    //    foreach (var eventO in schedule.Events)
                    //    {
                    //        var program = apiObj.Programs.FirstOrDefault(m => m.programId == eventO.ProgramId || m.Id == eventO.ProgramId);
                    //        var type = program.Type;

                    //        //la fecha de cancelacion es el dia siguiente al endDate, a las 23:59:59
                    //        DateTime cancelTo = eventO.EndDate.AddDays(1);
                    //        cancelTo = new DateTime(cancelTo.Year, cancelTo.Month, cancelTo.Day, 23, 59, 59);

                    //        var eventSchedule = new EventScheduleInformationRequestType()
                    //        {
                    //            ActionType = ActionTypeEnumeration.Insert,
                    //            EventScheduleInformation = new EventScheduleInformation()
                    //            {
                    //                EventPriceTierName = "Tier 8",
                    //                EventSchedule = new EventSchedule()
                    //                {
                    //                    Active = true,
                    //                    ActiveSpecified = true,
                    //                    AllowCancelFromDate = DateTime.Now, // al parecer es la fecha de carga de los ppv
                    //                    AllowCancelFromDateSpecified = true,
                    //                    AllowCancelFromTime = DateTime.Now, // al parecer es la fecha de carga de los ppv
                    //                    AllowCancelFromTimeSpecified = true,
                    //                    AllowCancelToDate = cancelTo, // revisar de donde sale
                    //                    AllowCancelToDateSpecified = true,
                    //                    AllowCancelToTime = cancelTo, // revisar de donde sale
                    //                    AllowCancelToTimeSpecified = true,
                    //                    AllowPackaging = false,
                    //                    ChannelId = channel.VcNumber,
                    //                    ChannelIdSpecified = true,
                    //                    EventCategoryId = 0, // se completa mas adelante
                    //                    EventEndDateTime = eventO.EndDate,
                    //                    EventEndDateTimeSpecified = true,
                    //                    EventProvisioningEntitlement = new EventProvisioningEntitlementCollection()
                    //                    {
                    //                        Items = new EventProvisioningEntitlement[1]
                    //                        {
                    //                    new EventProvisioningEntitlement()
                    //                    {
                    //                        Active = true,
                    //                        ActiveSpecified=true,
                    //                        DefaultEntitlement = "", //revisar de donde sale
                    //                        Entitlement2 ="", //se completa mas adelante
                    //                        Entitlement3 = "", //revisar de donde sale
                    //                    }
                    //                        },
                    //                        More = false,
                    //                        Page = 0,
                    //                    },
                    //                    EventScheduleExternalId = eventO.VrioId ?? eventO.Id,
                    //                    EventStartDateTime = eventO.StartDate,
                    //                    EventStartDateTimeSpecified = true,
                    //                    EventSubCategoryId = 0, // se completa mas adelante
                    //                    FreeText = "", // se completa mas adelante
                    //                    OnDemand = false,
                    //                    OnDemandSpecified = true,
                    //                    SellFromDateTime = DateTime.Now, //al parecer es la fecha de carga de los ppv
                    //                    SellFromDateTimeSpecified = true,
                    //                    SellToDateTime = eventO.EndDate,
                    //                    SellToDateTimeSpecified = true,
                    //                    ViewPriceTierId = 0, // se completa mas adelante
                    //                    Extended = eventO.Id
                    //                },

                    //                EventTitle = new EventTitle()
                    //                {
                    //                    Active = true,
                    //                    ActiveSpecified = true,
                    //                    AdditionalInfoURL = "http://www.directvla.com",
                    //                    Duration = eventO.Duration.ToString(),
                    //                    EventCategoryId = 0, // se completa mas adelante
                    //                    EventSubCategoryId = 0, // se completa mas adelante
                    //                    EventTitleDescription = "", //se completa mas adelante
                    //                    EventTitleExternalId = eventO.ProgramId,
                    //                    ImportFileId = 0,
                    //                    LanguageId1 = "",
                    //                    LanguageId2 = "",
                    //                    LanguageId3 = "",
                    //                    LanguageId4 = "",
                    //                    LanguageInvoiceText1 = "",
                    //                    LanguageInvoiceText2 = "",
                    //                    LanguageInvoiceText3 = "",
                    //                    LanguageInvoiceText4 = "",
                    //                    Name = ""
                    //                },
                    //                Extended = "0",
                    //            }
                    //        };

                    //        if (eventO.PurchaseInfo is not null)
                    //        {
                    //            var tier = apiObj.Tiers.FirstOrDefault(m => m.TierName == eventO.PurchaseInfo.TierPrice); // hay que confirmar que ID
                    //            if (tier is not null)
                    //            {
                    //                eventSchedule.EventScheduleInformation.EventPriceTierName = $"Tier {tier.TierName}";
                    //            }
                    //            else
                    //            {
                    //                eventSchedule.EventScheduleInformation.EventPriceTierName = $"{eventO.PurchaseInfo.TierPrice}";
                    //            }
                    //            eventSchedule.EventScheduleInformation.EventSchedule.ViewPriceTierId = eventO.PurchaseInfo.TierPrice.ToInt();
                    //            eventSchedule.EventScheduleInformation.EventSchedule.ViewPriceTierIdSpecified = true;
                    //            eventSchedule.EventScheduleInformation.EventSchedule.EventProvisioningEntitlement.Items[0].Entitlement2 = eventO.PurchaseInfo.IppvId.ToString();
                    //        }
                    //        if (program.Titles is not null)
                    //        {
                    //            int indice = 1;
                    //            foreach (var programTitles in program.Titles)
                    //            {
                    //                switch (indice)
                    //                {
                    //                    case 1:
                    //                        eventSchedule.EventScheduleInformation.EventTitle.LanguageId1 = programTitles.Lang.Substring(0, 1).ToUpper();
                    //                        eventSchedule.EventScheduleInformation.EventTitle.LanguageInvoiceText1 = programTitles.Value;

                    //                        eventSchedule.EventScheduleInformation.EventTitle.Name = programTitles.Value;
                    //                        break;
                    //                    case 2:
                    //                        eventSchedule.EventScheduleInformation.EventTitle.LanguageId2 = programTitles.Lang.Substring(0, 1).ToUpper();
                    //                        eventSchedule.EventScheduleInformation.EventTitle.LanguageInvoiceText2 = programTitles.Value;
                    //                        break;
                    //                    case 3:
                    //                        eventSchedule.EventScheduleInformation.EventTitle.LanguageId3 = programTitles.Lang.Substring(0, 1).ToUpper();
                    //                        eventSchedule.EventScheduleInformation.EventTitle.LanguageInvoiceText3 = programTitles.Value;
                    //                        break;
                    //                    case 4:
                    //                        eventSchedule.EventScheduleInformation.EventTitle.LanguageId4 = programTitles.Lang.Substring(0, 1).ToUpper();
                    //                        eventSchedule.EventScheduleInformation.EventTitle.LanguageInvoiceText4 = programTitles.Value;
                    //                        break;

                    //                    default:
                    //                        break;
                    //                }
                    //                indice++;
                    //            }
                    //        }
                    //        if (program.Descriptions is not null)
                    //        {
                    //            int indice = 1;
                    //            foreach (var programDescriptions in program.Descriptions)
                    //            {
                    //                switch (indice)
                    //                {
                    //                    default:
                    //                        eventSchedule.EventScheduleInformation.EventTitle.EventTitleDescription = programDescriptions.Value;
                    //                        continue;
                    //                }
                    //            }
                    //        }

                    //        if (program.Descriptions is not null)
                    //        {
                    //            int indice = 1;
                    //            foreach (var programDescriptions in program.Descriptions)
                    //            {
                    //                switch (indice)
                    //                {
                    //                    default:
                    //                        eventSchedule.EventScheduleInformation.EventTitle.EventTitleDescription = programDescriptions.Value;
                    //                        continue;
                    //                }
                    //            }
                    //        }
                    //        _logger.LogInformation($"   evento {eventSchedule.EventScheduleInformation.EventTitle.Name}");



                    //        list.Add(eventSchedule);
                    //    }

                    //    UpdateItem.EventScheduleInformationCollection = list;
                    //    requestBBVOD.items.Add(UpdateItem);
                    //}
                }
            }


        }

        private void bCerrar_Click(object sender, EventArgs e)
        {
            treeView1.CollapseAll();
        }

        private void bMostrarConPrecio_Click(object sender, EventArgs e)
        {
            treeView1.CollapseAll();

            //foreach (TreeNode item in treeView1.TopNode.Nodes)
            //{
            //    foreach (TreeNode item2 in item.Nodes)
            //    {
            //        if (item2.NodeFont != null && item2.NodeFont.Bold)
            //        {                        
            //            item2.Parent.Expand();
            //            item2.Parent.Parent.Expand();
            //            //item2.EnsureVisible();
            //        }
            //    }
            //}
            foreach (TreeNode item in treeView1.Nodes)
            {
                if (item.NodeFont != null && item.NodeFont.Bold)
                {
                    item.Expand();
                }
            }
        }

        private void bAbrir_Click(object sender, EventArgs e)
        {
            treeView1.ExpandAll();
        }
    }
}
