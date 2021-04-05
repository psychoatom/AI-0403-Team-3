using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WebMaskDetection.EntityStore;
using WebMaskDetection.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Diagnostics;
using Newtonsoft.Json;
using System.Net;
using System.Net.Mail;
using Microsoft.EntityFrameworkCore;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using MimeKit.Text;


namespace WebMaskDetection.Controllers
{
    public class WebCamController : Controller
    {
        private readonly DatabaseContext _context;
        private readonly IHostingEnvironment _environment;
        public WebCamController(IHostingEnvironment hostingEnvironment, DatabaseContext context)
        {
            _environment = hostingEnvironment;
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Capture()
        {
            int webCamInterval = await _context.WebCamPredictSetting.Select(x => x.WebCamInterval).FirstAsync();
            ViewData["webCamInterval"] = webCamInterval;
            return View(await _context.LocationSetting.ToListAsync());
        }


        [HttpPost]
        public async Task<IActionResult> Capture(string locationAndIndex)
        {
            string[] locationModel = locationAndIndex.Split(";");
            string location = locationModel[1];
            int locationIndex = Convert.ToInt32(locationModel[0]);
            ///Debug.Write("test");
            Debug.WriteLine("test");
            
            
                FileInfo fileToDelete;
            //set a dummy file
            string directoryPath = Path.Combine(_environment.WebRootPath, "CameraPhotos");
            string filepath = directoryPath + "\a.jpg";
            try
            {

                string locationStr = location;
                var files = HttpContext.Request.Form.Files;

                if (files != null)
                {
                    foreach (var file in files)
                    {
                        if (file.Length > 0)
                        {
                            // Getting Filename
                            var fileName = file.FileName;
                            // Unique filename "Guid"
                            //var myUniqueFileName = Convert.ToString(Guid.NewGuid());
                            var myUniqueFileName = DateTime.Now.ToString("yyyymmddMMss");
                            // Getting Extension
                            var fileExtension = Path.GetExtension(fileName);
                            // Concating filename + fileExtension (unique filename)
                            var newFileName = string.Concat(myUniqueFileName, fileExtension);
                            //  Generating Path to store photo 
                            filepath = Path.Combine(_environment.WebRootPath, "CameraPhotos") + $@"\{newFileName}";

                            if (!string.IsNullOrEmpty(filepath))
                            {
                                // Storing Image in Folder
                                StoreInFolder(file, filepath);
                                //location str is the option choosen by user
                                await MakePredictionRequest(filepath, locationStr, locationIndex);
                                fileToDelete = new FileInfo(filepath);
                                if (fileToDelete.Exists)
                                {
                                    fileToDelete.Delete();
                                }
                            }

                            ///var imageBytes = System.IO.File.ReadAllBytes(filepath);
                            ///if (imageBytes != null)
                            ///{
                            // Storing Image in Folder
                            /// StoreInDatabase(imageBytes);
                            ///}

                        }
                    }
                    return Json(true);
                }
                else
                {
                    return Json(false);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Capture procedure");
                Debug.WriteLine(ex.InnerException);
                return Json(false);
            }
            finally
            {
            }

        }

        /// <summary>
        /// Saving captured image into Folder.
        /// </summary>
        /// <param name="file"></param>
        /// <param name="fileName"></param>
        private void StoreInFolder(IFormFile file, string fileName)
        {
            using (FileStream fs = System.IO.File.Create(fileName))
            {
                file.CopyTo(fs);
                fs.Flush();
                fs.Close();
            }
        }

        /// <summary>
        /// Saving captured image into database.
        /// </summary>
        /// <param name="imageBytes"></param>
        private void StoreInDatabase(byte[] imageBytes)
        {
            try
            {
                if (imageBytes != null)
                {
                    string base64String = Convert.ToBase64String(imageBytes, 0, imageBytes.Length);
                    string imageUrl = string.Concat("data:image/jpg;base64,", base64String);

                    ImageStore imageStore = new ImageStore()
                    {
                        CreateDate = DateTime.Now,
                        ImageBase64String = imageUrl,
                        ImageId = 0
                    };

                    _context.ImageStore.Add(imageStore);
                    _context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Capture procedure");
                Debug.WriteLine(ex.InnerException);
            }
        }
        public async Task MakePredictionRequest(string imageFilePath, string locationStr, int locationIndex)
        {

            try
            {   // get the prediction proability threshold to send email or telegram from the database
                double predicationThreshold = decimal.ToDouble(_context.WebCamPredictSetting.Select(x => x.PredictionThreshold).FirstOrDefault());

                var client = new HttpClient();

                // Request headers - replace this example key with your valid Prediction-Key.
                client.DefaultRequestHeaders.Add("Prediction-Key", "************");

                // Prediction URL - replace this example URL with your valid Prediction URL.
                string url = "https://aiproject.cognitiveservices.azure.com/customvision/v3.0/Prediction/ecc60d7c-b3c8-4b65-9e60-1ae67ccb2ae4/classify/iterations/Iteration2/image";

                HttpResponseMessage response;
                double proValue;
                string strProValue;
                // Request body. Try this sample with a locally stored image.
                byte[] byteData = await GetImageAsByteArray(imageFilePath);

                var content = new ByteArrayContent(byteData);
                
                    content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                    response = await client.PostAsync(url, content);
                    string text = await response.Content.ReadAsStringAsync();
                    dynamic predication = JsonConvert.DeserializeObject(text);
                    foreach (var pred in predication.predictions)
                    {
                        string tagname = pred.tagName;
                        if (tagname == "NoMask")
                        {
                            strProValue = pred.probability;
                            double.TryParse(strProValue, out proValue);

                            if (double.IsNaN(proValue) == false && double.IsInfinity(proValue) == false)
                            { if (proValue > predicationThreshold)
                                {
                                IncidentReport incidentReport = new IncidentReport();
                                incidentReport.LocationIndex = locationIndex;
                                incidentReport.IncidentDateTime = DateTime.Now;
                                _context.Add(incidentReport);
                                await _context.SaveChangesAsync();
                                    //await sentEmail(imageFilePath, locationStr, locationIndex);
                                    await sentEmailMailKit(imageFilePath, locationStr, locationIndex);
                                await sentTelegram(imageFilePath, locationStr, locationIndex);
                                }
                            }
                        }
                    }

               
            } catch (Exception ex)
            {
                Debug.WriteLine("Made Prediction");
                Debug.WriteLine(ex.InnerException);
            }
        }
        public async Task saveIncident(string locationIndex)
        {
            
        }
        public async Task sentEmailMailKit(string filename, string locationStr, int locationIndex)
        {
            
            List<string> emailList = _context.LocationEmail.Where(x => x.LocationIndex == locationIndex).Select(x => x.Email).ToList();
           try {  

            foreach (string emailStr in emailList)
            {
                var email = new MimeMessage();
                email.From.Add(new MailboxAddress("SkyTech","*****@gmail.com"));
                email.To.Add(new MailboxAddress("SecurityStaff",emailStr));
                //email.To.Add(MailboxAddress.Parse("to_address@example.com"));
                email.Subject = "Notification of people without mask";
               
                string bodyText = "Date Time: " + DateTime.Now + " < br /> ";
                bodyText = bodyText + "Location: " + locationStr + "<br/><br/>";
                bodyText = bodyText + "A person not wearing mask is discovered in the premises.<br/> This is a system generated email. Please do not reply.";

                var builder = new BodyBuilder();
                builder.HtmlBody = string.Format(@bodyText);

                //var image = builder.LinkedResources.Add(@filename);
                builder.Attachments.Add(@filename);
                email.Body = builder.ToMessageBody();
                // send email 

                using (var smtp = new MailKit.Net.Smtp.SmtpClient())
            {
                smtp.Connect("***.com", 587, SecureSocketOptions.None);
                smtp.Authenticate("***@***.com", "******");
                await smtp.SendAsync(email);
                smtp.Disconnect(true);
            }
            }
        }
            catch (Exception ex)
            {
                Debug.WriteLine("Sent Email");
                Debug.WriteLine(ex.InnerException);
            }
            finally
            {

            }
        }
        public async Task sentEmail(string filename, string locationStr, int locationIndex)
        {

            try
            {
                System.Net.Mail.Attachment attachment = new System.Net.Mail.Attachment(filename, System.Net.Mime.MediaTypeNames.Text.Html);


                var msg = new MailMessage();
                List<string> emailList = _context.LocationEmail.Where(x => x.LocationIndex == locationIndex).Select(x => x.Email).ToList();
                string emailListString = "";
                foreach (string email in emailList)
                {
                    msg.To.Add(new MailAddress(email));
                }

                var credentialUserName = "*****@*****.com";
                var sentFrom = "*****@gmail.com";
                var pwd = "**********";
                
                var smtp = new System.Net.Mail.SmtpClient("****.com", 587);


                var creds = new System.Net.NetworkCredential(credentialUserName, pwd);

                smtp.UseDefaultCredentials = false;
                smtp.Credentials = creds;
                smtp.EnableSsl = false;
               
                //var to = new MailAddress(emailListString);
                var from = new MailAddress(sentFrom, "SkyTech Mask Monitoring System");

                //msg.To.Add(to);
                msg.From = from;
                msg.IsBodyHtml = true;
                msg.Subject = "Notification of people without mask";
                msg.Body = "Date Time: " + DateTime.Now + "<br/>";
                msg.Body = msg.Body + "Location: " + locationStr + "<br/><br/>";
                msg.Body = msg.Body + "A person not wearing mask is discovered in the premises.<br/> This is a system generated email. Please do not reply.";
                if (attachment != null)
                { msg.Attachments.Add(attachment); }
                await smtp.SendMailAsync(msg);
                smtp.Dispose();





            }
            catch (Exception ex)
            {
                Debug.WriteLine("Sent Email");
                Debug.WriteLine(ex.InnerException);
            }
            finally
            {

            }
        }

        private async Task<byte[]> GetImageAsByteArray(string imageFilePath)
        {
            FileStream fileStream = new FileStream(imageFilePath, FileMode.Open, FileAccess.Read);
            BinaryReader binaryReader = new BinaryReader(fileStream);
            return binaryReader.ReadBytes((int)fileStream.Length);
        }
        public async Task sentTelegram(string filename, string locationStr, int locationIndex)
        {

            try
            {
                var bot = new Telegram.Bot.TelegramBotClient("*******************");
                List<int> ChatIDList = _context.LocationTelegram.Where(x => x.LocationIndex == locationIndex).Select(x => x.ChatID).ToList();

                foreach (int chatID in ChatIDList)
                {


                    await bot.SendTextMessageAsync(chatID, "From: SkyTech Mask Monitoring System");
                    await bot.SendTextMessageAsync(chatID, "Subject: Notification of people without mask");
                    await bot.SendTextMessageAsync(chatID, "Location: " + locationStr);
                    await bot.SendTextMessageAsync(chatID, "Date Time: " + DateTime.Now);
                    await bot.SendTextMessageAsync(chatID, "A person not wearing mask is discovered in the premises.<br/> This is a system generated email. Please do not reply.");
                    using (Stream stream = System.IO.File.OpenRead(filename))
                    {
                        await bot.SendPhotoAsync(chatID, stream);
                    }
                }
            } catch (Exception ex)
            {
                Debug.WriteLine("Send telegram");
                Debug.WriteLine(ex.InnerException);

            }

        }
    }

 }