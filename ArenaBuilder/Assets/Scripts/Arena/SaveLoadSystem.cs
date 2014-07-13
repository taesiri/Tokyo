using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml;
using Assets.Scripts.Helpers;

namespace Assets.Scripts.Arena
{
    public static class SaveLoadSystem
    {
        public static void SaveDataToXML(this AdvanceGrid gameGrid, string mapName)
        {
            Deployable[] tiles = gameGrid.GetAllChildren();

            using (XmlWriter writer = XmlWriter.Create(@"D:\data.xml")) //Application.persistentDataPath + "/" + mapName + ".dm"
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("Tiles");
                writer.WriteAttributeString("Row", gameGrid.Rows.ToString(CultureInfo.InvariantCulture));
                writer.WriteAttributeString("Column", gameGrid.Columns.ToString(CultureInfo.InvariantCulture));

                foreach (Deployable tile in tiles)
                {
                    writer.WriteStartElement("Tile");

                    writer.WriteAttributeString("Name", tile.GetDisplayName());
                    writer.WriteAttributeString("GridIndex", tile.GridIndex.ToString());


                    if (tile.HasChanged)
                    {
                        writer.WriteStartElement("Properties");

                        //Boolean Properties

                        List<GamePropertyWithName> bList = tile.GetPropertyOfType(typeof (bool));

                        foreach (GamePropertyWithName booleanProperty in bList)
                        {
                            writer.WriteStartElement("Property");
                            writer.WriteAttributeString("Name", booleanProperty.PropertyName);
                            writer.WriteAttributeString("Type", "bool");
                            writer.WriteAttributeString("Value", ((bool) booleanProperty.GamePropertyInfo.GetValue(tile, null)).ToString());
                            writer.WriteEndElement();
                        }


                        //String Properties

                        List<GamePropertyWithName> sList = tile.GetPropertyOfType(typeof (string));

                        foreach (GamePropertyWithName stringProperty in sList)
                        {
                            writer.WriteStartElement("Property");
                            writer.WriteAttributeString("Name", stringProperty.PropertyName);
                            writer.WriteAttributeString("Type", "string");
                            writer.WriteAttributeString("Value", (string) stringProperty.GamePropertyInfo.GetValue(tile, null));
                            writer.WriteEndElement();
                        }


                        //String Properties

                        List<GamePropertyWithName> intList = tile.GetPropertyOfType(typeof (int));

                        foreach (GamePropertyWithName integerProperty in intList)
                        {
                            writer.WriteStartElement("Property");
                            writer.WriteAttributeString("Name", integerProperty.PropertyName);
                            writer.WriteAttributeString("Type", "int");
                            writer.WriteAttributeString("Value", ((int) integerProperty.GamePropertyInfo.GetValue(tile, null)).ToString());
                            writer.WriteEndElement();
                        }


                        //String Properties

                        List<GamePropertyWithName> fList = tile.GetPropertyOfType(typeof (float));

                        foreach (GamePropertyWithName floatProperty in fList)
                        {
                            writer.WriteStartElement("Property");
                            writer.WriteAttributeString("Name", floatProperty.PropertyName);
                            writer.WriteAttributeString("Type", "float");
                            writer.WriteAttributeString("Value", ((float) floatProperty.GamePropertyInfo.GetValue(tile, null)).ToString());
                            writer.WriteEndElement();
                        }
                        writer.WriteEndElement();
                    }

                    writer.WriteEndElement();
                }

                writer.WriteEndElement();
                writer.WriteEndDocument();
            }
        }

        public static void LoadDataFromXML(this AdvanceGrid gameGrid, string mapName, Dictionary<string, Deployable> deployableDictionary)
        {
            var reader = new XmlDocument();
            reader.Load(@"D:\data.xml");
            //reader.Load(Application.persistentDataPath + "/" + mapName + ".dm");

            if (reader.DocumentElement != null)
            {
                int row = Convert.ToInt32(reader.DocumentElement.Attributes["Row"].InnerText);
                int column = Convert.ToInt32(reader.DocumentElement.Attributes["Column"].InnerText);

                if (gameGrid.Rows == row && gameGrid.Columns == column)
                {
                    foreach (XmlNode node in reader.DocumentElement.ChildNodes)
                    {
                        if (node.Attributes != null)
                        {
                            var index = new IntVector2(node.Attributes["GridIndex"].InnerText);
                            string objectName = node.Attributes["Name"].InnerText;


                            Deployable newTile = gameGrid.DeployIfPossible(index, deployableDictionary[objectName]);
                            if (newTile)
                            {
                                //Applying Properties!
                                XmlNode proeprties = node.ChildNodes[0];
                                if (proeprties != null)
                                {
                                    foreach (XmlNode proeprty in proeprties.ChildNodes)
                                    {
                                        if (proeprty.Attributes != null)
                                        {
                                            string typeOfProperty = proeprty.Attributes["Type"].InnerText;
                                            Type t = null;
                                            switch (typeOfProperty)
                                            {
                                                case "bool":
                                                    t = typeof (bool);
                                                    break;

                                                case "string":
                                                    t = typeof (string);
                                                    break;

                                                case "int":
                                                    t = typeof (int);
                                                    break;

                                                case "float":
                                                    t = typeof (float);
                                                    break;
                                            }
                                            if (t != null)
                                                newTile.SetProperty(t, proeprty.Attributes["Name"].InnerText, proeprty.Attributes["Value"].InnerText);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    UpdateGridSize(gameGrid);
                }
            }
        }

        public static void UpdateGridSize(this AdvanceGrid gameGrid)
        {
            //TODO : Change Grid Size
        }
    }
}