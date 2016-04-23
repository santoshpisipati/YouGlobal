using GlobalPanda.BusinessInfo;
using ICSharpCode.SharpZipLib.Zip;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Xml;

/// <summary>
/// Summary description for EmailDataProvider
/// </summary>

namespace GlobalPanda.DataProviders
{
    public class FileDataProvider
    {
        public static uint insertFile(Stream fileStream, string name)
        {
            long size = (long)(fileStream.Length / 1024);

            string sql = "insert into files (name, size) values (?name, ?size); select last_insert_id()";
            uint fileid = Convert.ToUInt32(DAO.ExecuteScalar(sql, new MySqlParameter("name", name), new MySqlParameter("size", size)));

            string filename = Common.Base2Base(fileid.ToString(), 10, 36);
            filename = filename.PadLeft(8, '0');
            //string path = System.Web.HttpContext.Current.Server.MapPath("~");
            //FileStream zipFile = File.OpenWrite(path + "\\..\\files\\" + filename + ".zip");
            string path = ConfigurationManager.AppSettings.Get("filePath");
            path = path + filename.Substring(0, 2) + "\\" + filename.Substring(2, 2) + "\\" + filename.Substring(4, 2) + "\\";
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
            FileStream zipFile = File.OpenWrite(path + filename + ".zip");
            ZipOutputStream zos = new ZipOutputStream(zipFile);
            zos.SetLevel(9);
            byte[] buffer = new byte[32768];

            ZipEntry ze = new ZipEntry(name);
            zos.PutNextEntry(ze);

            fileStream.Position = 0;
            long len = fileStream.Length;
            while (len > 0)
            {
                int readSoFar = fileStream.Read(buffer, 0, buffer.Length);
                zos.Write(buffer, 0, readSoFar);
                len -= readSoFar;
            }
            zos.CloseEntry();
            zos.Finish();
            zos.Close();

            sql = "update files set filename = ?filename where fileid = ?fileid";
            DAO.ExecuteNonQuery(sql, new MySqlParameter("fileid", fileid), new MySqlParameter("filename", filename));

            return fileid;
        }

        public static MySqlDataReader getFile(uint fileid)
        {
            string sql = "select f.fileid, f.name, f.size, f.filename, cf.uploaded, ft.name as filetype, f.size,cf.filetypeid " +
                "from candidates_files as cf " +
                "join files as f on f.fileid = cf.fileid " +
                "join filetypes as ft on ft.filetypeid = cf.filetypeid  where f.fileid = ?fileid";
            return DAO.ExecuteReader(sql, new MySqlParameter("fileid", fileid));
        }

        public static MySqlDataReader getFileOnly(uint fileid)
        {
            string sql = "select f.fileid, f.name, f.size, f.filename " +
                "from  files as f  where f.fileid = ?fileid";
            return DAO.ExecuteReader(sql, new MySqlParameter("fileid", fileid));
        }

        public static string parseDoc(Stream fileStream)
        {
            string text = null;
            string path = ConfigurationManager.AppSettings.Get("tempPath");
            //string filename = System.Web.HttpContext.Current.Server.MapPath("~") + "\\..\\temp\\" + Guid.NewGuid().ToString() + ".doc";
            string filename = path + Guid.NewGuid().ToString() + ".doc";
            FileStream writer = new FileStream(filename, FileMode.Create, FileAccess.Write);
            int len = 10240;
            Byte[] buffer = new Byte[len];
            int read = fileStream.Read(buffer, 0, len);
            // write the required bytes
            while (read > 0)
            {
                writer.Write(buffer, 0, read);
                read = fileStream.Read(buffer, 0, len);
            }
            writer.Close();
            DIaLOGIKa.b2xtranslator.StructuredStorage.Reader.StructuredStorageReader reader = null;
            try
            {
                reader = new DIaLOGIKa.b2xtranslator.StructuredStorage.Reader.StructuredStorageReader(filename);
                DIaLOGIKa.b2xtranslator.DocFileFormat.WordDocument doc = new DIaLOGIKa.b2xtranslator.DocFileFormat.WordDocument(reader);
                text = new String(doc.Text.ToArray());
            }
            catch (DIaLOGIKa.b2xtranslator.DocFileFormat.ByteParseException bpe)
            {
                text = "Unable_to_read_DOC_Document";
            }
            catch (DIaLOGIKa.b2xtranslator.StructuredStorage.Common.InvalidValueInHeaderException ivihe)
            {
                text = "Unable_to_open_DOC_Document";
            }
            finally
            {
                if (reader != null) reader.Close();
            }

            File.Delete(filename);
            text = text.Replace("\t", " ").Replace("\n", " ").Replace("\r", " ");
            return text;
        }

        public static string parseDocx(Stream fileStream)
        {
            //string filename = System.Web.HttpContext.Current.Server.MapPath("~") + "\\..\\temp\\" + Guid.NewGuid().ToString() + ".docx";
            string path = ConfigurationManager.AppSettings.Get("tempPath");
            string filename = path + Guid.NewGuid().ToString() + ".docx";
            FileStream writer = new FileStream(filename, FileMode.Create, FileAccess.Write);
            int len = 10240;
            Byte[] buffer = new Byte[len];
            int read = fileStream.Read(buffer, 0, len);
            // write the required bytes
            while (read > 0)
            {
                writer.Write(buffer, 0, read);
                read = fileStream.Read(buffer, 0, len);
            }
            writer.Close();
            writer.Dispose();
            writer = null;

            DocxToText docx = new DocxToText(filename);
            string text = docx.ExtractText();

            File.Delete(filename);

            text = text.Replace("\t", " ").Replace("\n", " ").Replace("\r", " ");
            return text;
        }

        public static string parsePdf(Stream fileStream)
        {
            //int size = (int)fileStream.Length;
            //Byte[] buffer = new Byte[size];
            //fileStream.Read(buffer, 0, size);
            //java.io.ByteArrayInputStream bais = new java.io.ByteArrayInputStream(buffer);
            //PDDocument doc = null;
            string text = null;
            //try
            //{
            //    doc = PDDocument.load(bais);
            //    if (!doc.isEncrypted())
            //    {
            //        try
            //        {
            //            PDFTextStripper stripper = new PDFTextStripper();
            //            text = stripper.getText(doc);
            //            text = text.Replace("\t", " ").Replace("\n", " ").Replace("\r", " ");
            //        }
            //        catch (NullReferenceException nre)
            //        {
            //            text = "Unable_to_read_PDF_Document";
            //        }
            //    }
            //    else
            //    {
            //        text = "Encrypted_PDF_Document";
            //    }
            //}
            //finally
            //{
            //    if (doc != null) doc.close();
            //}
            return text;
        }

        public static string parseTxt(Stream fileStream)
        {
            StreamReader sr = new StreamReader(fileStream);
            return sr.ReadToEnd();
        }

        public static string parseRtf(Stream fileStream)
        {
            RichTextBox rtb = new RichTextBox();
            rtb.LoadFile(fileStream, RichTextBoxStreamType.RichText);
            return rtb.Text;
        }

        public static MySqlDataReader listFileTypes(uint tenantid)
        {
            string sql = "select ft.filetypeid, ft.name " +
                         "from filetypes as ft " +
                         "order by ft.name";
            return DAO.ExecuteReader(sql);
        }

        private class DocxToText
        {
            private const string ContentTypeNamespace =
                @"http://schemas.openxmlformats.org/package/2006/content-types";

            private const string WordprocessingMlNamespace =
                @"http://schemas.openxmlformats.org/wordprocessingml/2006/main";

            private const string DocumentXmlXPath = "/t:Types/t:Override[@ContentType=\"application/vnd.openxmlformats-officedocument.wordprocessingml.document.main+xml\"]";

            private const string BodyXPath = "/w:document/w:body";

            private string docxFile = "";
            private string docxFileLocation = "";

            public DocxToText(string fileName)
            {
                docxFile = fileName;
            }

            #region ExtractText()

            ///
            /// Extracts text from the Docx file.
            ///
            /// Extracted text.
            public string ExtractText()
            {
                if (string.IsNullOrEmpty(docxFile))
                    throw new Exception("Input file not specified.");

                // Usually it is "/word/document.xml"

                docxFileLocation = FindDocumentXmlLocation();

                if (string.IsNullOrEmpty(docxFileLocation))
                    throw new Exception("It is not a valid Docx file.");

                return ReadDocumentXml();
            }

            #endregion ExtractText()

            #region FindDocumentXmlLocation()

            ///
            /// Gets location of the "document.xml" zip entry.
            ///
            /// Location of the "document.xml".
            private string FindDocumentXmlLocation()
            {
                string found = null;
                ZipFile zip = new ZipFile(docxFile);
                foreach (ZipEntry entry in zip)
                {
                    // Find "[Content_Types].xml" zip entry

                    if (string.Compare(entry.Name, "[Content_Types].xml", true) == 0)
                    {
                        Stream contentTypes = zip.GetInputStream(entry);

                        XmlDocument xmlDoc = new XmlDocument();
                        xmlDoc.PreserveWhitespace = true;
                        xmlDoc.Load(contentTypes);
                        contentTypes.Close();

                        //Create an XmlNamespaceManager for resolving namespaces

                        XmlNamespaceManager nsmgr =
                            new XmlNamespaceManager(xmlDoc.NameTable);
                        nsmgr.AddNamespace("t", ContentTypeNamespace);

                        // Find location of "document.xml"

                        XmlNode node = xmlDoc.DocumentElement.SelectSingleNode(
                            DocumentXmlXPath, nsmgr);

                        if (node != null)
                        {
                            string location =
                                ((XmlElement)node).GetAttribute("PartName");
                            found = location.TrimStart(new char[] { '/' });
                        }
                        break;
                    }
                }

                zip.Close();
                return found;
            }

            #endregion FindDocumentXmlLocation()

            #region ReadDocumentXml()

            ///
            /// Reads "document.xml" zip entry.
            ///
            /// Text containing in the document.
            private string ReadDocumentXml()
            {
                StringBuilder sb = new StringBuilder();

                ZipFile zip = new ZipFile(docxFile);
                foreach (ZipEntry entry in zip)
                {
                    if (string.Compare(entry.Name, docxFileLocation, true) == 0)
                    {
                        Stream documentXml = zip.GetInputStream(entry);

                        XmlDocument xmlDoc = new XmlDocument();
                        xmlDoc.PreserveWhitespace = true;
                        xmlDoc.Load(documentXml);
                        documentXml.Close();

                        XmlNamespaceManager nsmgr =
                            new XmlNamespaceManager(xmlDoc.NameTable);
                        nsmgr.AddNamespace("w", WordprocessingMlNamespace);

                        XmlNode node =
                            xmlDoc.DocumentElement.SelectSingleNode(BodyXPath, nsmgr);

                        if (node == null)
                            return string.Empty;

                        sb.Append(ReadNode(node));

                        break;
                    }
                }
                zip.Close();
                return sb.ToString();
            }

            #endregion ReadDocumentXml()

            #region ReadNode()

            ///
            /// Reads content of the node and its nested childs.
            ///
            /// XmlNode.
            /// Text containing in the node.
            private string ReadNode(XmlNode node)
            {
                if (node == null || node.NodeType != XmlNodeType.Element)
                    return string.Empty;

                StringBuilder sb = new StringBuilder();
                foreach (XmlNode child in node.ChildNodes)
                {
                    if (child.NodeType != XmlNodeType.Element) continue;

                    switch (child.LocalName)
                    {
                        case "t":                           // Text
                            sb.Append(child.InnerText.TrimEnd());

                            string space =
                                ((XmlElement)child).GetAttribute("xml:space");
                            if (!string.IsNullOrEmpty(space) &&
                                space == "preserve")
                                sb.Append(' ');

                            break;

                        case "cr":                          // Carriage return
                        case "br":                          // Page break
                            sb.Append(Environment.NewLine);
                            break;

                        case "tab":                         // Tab
                            sb.Append("\t");
                            break;

                        case "p":                           // Paragraph
                            sb.Append(ReadNode(child));
                            sb.Append(Environment.NewLine);
                            sb.Append(Environment.NewLine);
                            break;

                        default:
                            sb.Append(ReadNode(child));
                            break;
                    }
                }
                return sb.ToString();
            }

            #endregion ReadNode()
        }

        public static bool isDuplicateFileType(string name, uint filetypeid)
        {
            string sql = "select count(filetypeid) as `exists` from filetypes where name = ?name and filetypeid != ?filetypeid";
            uint exists = Convert.ToUInt32(DAO.ExecuteScalar(sql, new MySqlParameter("name", name), new MySqlParameter("filetypeid", filetypeid)));
            return (exists > 0);
        }

        public static int changeFileRequest(uint fileId, uint candidateid, Int16 changeType, string changename, string oldname)
        {
            string sql = "insert into file_change (fileid,candidateid,changetype,oldname,changename,userid,requested,status) values(?fileid,?candidateid,?changetype,?oldname,?changename,?userid,?requested,?status);select last_insert_id() ";
            int id = Convert.ToInt32(DAO.ExecuteScalar(sql, new MySqlParameter("fileid", fileId), new MySqlParameter("candidateid", candidateid), new MySqlParameter("changetype", changeType),
                new MySqlParameter("oldname", oldname), new MySqlParameter("changename", changename), new MySqlParameter("userid", GPSession.UserId), new MySqlParameter("requested", DateTime.UtcNow), new MySqlParameter("status", 1)));
            return id;
        }

        public static MySqlDataReader getFilechangeRequest(uint fileId)
        {
            string sql = "select fileid,candidateid,changetype,changename,userid,requested,status,date_format(modified,'%d-%b-%Y-%T') as modified,replacefileid from file_change where fileid=?fileid";

            return DAO.ExecuteReader(sql, new MySqlParameter("fileid", fileId));
        }

        public static MySqlDataReader getFilechangeRequestById(int fileChangeId)
        {
            string sql = "select fc.fileid,candidateid,changetype,oldname,changename,fc.userid,requested,status,u.username as modifieduser, " +
                "case when changetype=2 then (select name from filetypes where filetypeid=changename) else changename end as newvalue," +
                "case when changetype=2 then (select name from filetypes where filetypeid=oldname) else oldname end as oldvalue,f.name as filename" +
                " from file_change fc inner join files f on fc.fileid=f.fileid left join users u on fc.modifieduser=u.userid where file_changeid=?filechangeid";

            return DAO.ExecuteReader(sql, new MySqlParameter("filechangeid", fileChangeId));
        }

        public static MySqlDataReader getAllFileChaneRequest()
        {
            string sql = "select file_changeid,fc.fileid,fc.candidateid,changetype,oldname,changename,fc.userid,date_format(requested,'%d-%b-%Y-%T') as requested,status,u.username, " +
                 "case when changetype=2 then (select name from filetypes where filetypeid=changename) else changename end as newvalue," +
                "case when changetype=2 then (select name from filetypes where filetypeid=oldname) else oldname end as oldvalue" +
                " from file_change fc left join users u on fc.userid=u.userid where status=1";

            return DAO.ExecuteReader(sql);
        }

        public static void updateChangeRequestStaus(int filechangeId, int status, uint candidateid, int fileid, string newvalue, int changetype, int? replacefileid = null)
        {
            string sql = "update file_change set status=?status,modified=?modified,modifieduser=?userid,replacefileid=?replacefileid where file_changeid=?filechangeId";
            DAO.ExecuteNonQuery(sql, new MySqlParameter("status", status), new MySqlParameter("filechangeId", filechangeId), new MySqlParameter("modified", DateTime.UtcNow), new MySqlParameter("userid", GPSession.UserId),
                new MySqlParameter("replacefileid", replacefileid));
            string columnname = string.Empty;
            if (status == 2)//approve
            {
                if (changetype == 1)
                {
                    columnname = "filename";
                    updateFilename(fileid, newvalue);
                }
                else if (changetype == 2)
                {
                    columnname = "filetype";
                    updateFiletype(fileid, Convert.ToInt32(newvalue));
                }
                else
                {
                    columnname = "delete";
                    updateFileDelete(fileid);
                }
            }

            HistoryDataProvider history = new HistoryDataProvider();
            HistoryInfo historyInfo = new HistoryInfo();
            historyInfo.UserId = GPSession.UserId;
            historyInfo.ModuleId = (int)HistoryInfo.Module.file_change;
            historyInfo.TypeId = (int)HistoryInfo.ActionType.Edit;
            historyInfo.RecordId = Convert.ToUInt32(filechangeId);
            historyInfo.ParentRecordId = candidateid;
            historyInfo.ModifiedDate = DateTime.Now;
            historyInfo.Details = new List<HistoryDetailInfo>();
            historyInfo.Details.Add(new HistoryDetailInfo { ColumnName = columnname, NewValue = status.ToString(), OldValue = "1" });
            history.insertHistory(historyInfo);
        }

        public static void updateFilename(int fileid, string filename)
        {
            string sql = "update files set name=?filename where fileid=?fileid";
            DAO.ExecuteNonQuery(sql, new MySqlParameter("fileid", fileid), new MySqlParameter("filename", filename));
        }

        public static void updateFiletype(int fileid, int filetype)
        {
            string sql = "update candidates_files set filetypeid=?filetype where fileid=?fileid";
            DAO.ExecuteNonQuery(sql, new MySqlParameter("fileid", fileid), new MySqlParameter("filetype", filetype));
        }

        public static void updateFileDelete(int fileid)
        {
            string sql = "update files set deleted=1 where fileid=?fileid";
            DAO.ExecuteNonQuery(sql, new MySqlParameter("fileid", fileid));
            string filename = string.Empty;
            string zipfilename = string.Empty;

            using (MySqlDataReader drFile = FileDataProvider.getFile(Convert.ToUInt32(fileid)))
            {
                if (drFile.Read())
                {
                    filename = DAO.getString(drFile, "name");
                    zipfilename = DAO.getString(drFile, "filename");
                }
            }
            string path = ConfigurationManager.AppSettings.Get("filePath");
            path = path + zipfilename.Substring(0, 2) + "\\" + zipfilename.Substring(2, 2) + "\\" + zipfilename.Substring(4, 2) + "\\";
            FileInfo fInfo = new FileInfo(path + zipfilename + ".zip");
            if (fInfo.Exists)
            {
                fInfo.Delete();
            }
        }
    }
}