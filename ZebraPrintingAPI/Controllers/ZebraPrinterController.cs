using System;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using ZebraPrintingAPI.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ZebraPrintingAPI.Controllers
{
    [Route("api/[controller]")]
    public class ZebraPrinterController : Controller
    {
        // POST api/values
        [HttpPost]
        public SuccessResponse PutByQuery([FromQuery]string IP, [FromBody]PrintContent content)
        {
            string zplString = content.ZPL;

            IPAddress ipAddress = IPAddress.Parse(IP);
            int port = 9100;
            IPEndPoint ipEndPoint = new IPEndPoint(ipAddress, port);
            Socket client = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            byte[] sendmsg = Encoding.UTF8.GetBytes(zplString);

            try
            {
                Ping pingSender = new Ping();
                PingReply reply = pingSender.Send(ipAddress);

                if (reply.Status != IPStatus.Success)
                {
                    SuccessResponse ping = new SuccessResponse
                    {
                        success = false,
                        message = reply.Status.ToString()
                    };
                    return ping;

                }
                else
                {
                    client.Connect(ipEndPoint);
                    int n = client.Send(sendmsg);
                    client.Shutdown(SocketShutdown.Both);
                    client.Close();
                    SuccessResponse response = new SuccessResponse
                    {
                        success = true,
                        message = zplString
                    };
                    return response;
                }
            }
            catch (Exception ex)
            {
                SuccessResponse errorResponse = new SuccessResponse
                {
                    success = false,
                    message = ex.ToString() + zplString
                };
                return errorResponse;
            }

        }

        // GET: api/values
        [HttpGet]
        public string Get()
        {
            return "Hello from the Zebra Printing Application";
        }

    }
}
