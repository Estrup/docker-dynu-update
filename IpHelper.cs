using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace DynuPdate
{
    public static class IpHelp
    {
        public static async Task<String> GetIp(){
            HttpClient client = new HttpClient();
            var ip = await client.GetStringAsync("http://ipecho.net/plain");
            return ip;
        }

        public static async Task UpdateIp(string hostname, string ip, string username, string password){
            HttpClient client = new HttpClient();
            var url = $"https://api.dynu.com/nic/update?hostname={ hostname }&myip={ ip }&username={username}&password={password}";
            var returnstring = await client.GetStringAsync(url);
        }
    }
}
