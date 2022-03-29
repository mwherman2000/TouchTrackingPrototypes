// Graph Engine
// BlueToqueTools
// Trusted Digial Web Project
// Hyperonomy Digital Identity Lab
// Parallelspace Corporation
// (c) Copyright 2021-2022 Parallelspace Corporation. All Rights Reserved

//////////////////////////////////////////////////////////////////////////////
/// BlueToqueTools Common Types

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO;

using Trinity;
using Trinity.Storage;
using System.Net.Http;

namespace BTTN4KNFE
{
    class NFELocalStorageAgentImplementation : LocalStorageAgentBase
    {
        private readonly long DIRECTORY_CELLID = 123456789;
        private static BTTN4KNFEDirectory Directory = new BTTN4KNFEDirectory(0, new List<long>());

        public NFELocalStorageAgentImplementation()
        {
            //Global.LocalStorage.LoadStorage();
            Global.LocalStorage.ResetStorage();
            if (Global.LocalStorage.Count() > 0)
            {
                Directory = Global.LocalStorage.LoadBTTN4KNFEDirectory(DIRECTORY_CELLID);
            }
            else
            {
                Global.LocalStorage.SaveBTTN4KNFEDirectory(DIRECTORY_CELLID, Directory);
                Global.LocalStorage.SaveStorage();
            }
        }

        public override void SendNFEByIdToLocalStorageHandler(SendNFEByIdRequestReader request, SendNFEByIdResponseWriter response)
        {
            using (request)
            {
                if (Directory.ids.Contains(request.id))
                {
                    // do nothing
                }
                else
                {
                    Directory.ids.Add(request.id);
                    Directory.count++;
                    Global.LocalStorage.SaveBTTN4KNFEDirectory(DIRECTORY_CELLID, Directory);
                    Global.LocalStorage.SaveStorage();
                }
            }
        }

        public override void GetNFELocalStorageCountHandler(GetNFECountRequestReader request, GetNFECountResponseWriter response)
        {
            response.count = Global.LocalStorage.LoadBTTN4KNFEDirectory(DIRECTORY_CELLID).count;
            response.rc = (int)TrinityErrorCode.E_SUCCESS;
        }

        public override void GetNFEFromLocalStorageHandler(GetNFERequestReader request, GetNFEResponseWriter response)
        {
            throw new NotImplementedException();
        }

        public override void GetNFEBatchFromLocalStorageHandler(GetNFEBatchRequestReader request, GetNFEBatchResponseWriter response)
        {
            throw new NotImplementedException();
        }

        public override void SendNFEEnvelopeToStorageHandler(SendNFEEnvelopeRequest request, out SendNFEEnvelopeResponse response)
        {
            BTTNFE_N4K_SealedEnvelope nfeSealedEnvelope = request.sealedEnvelope;
            string json = nfeSealedEnvelope.ToString();
            Console.WriteLine("nfeSealedEnvelope:\n" + json);
            File.WriteAllText("c:\\temp\\nfeSealedEnvelope.json", json);

            response.rc = (int)TrinityErrorCode.E_SUCCESS;
        }
    }

    class Program
    {
        private const int MAX_METRIC = 150;

        static readonly System.Reflection.Assembly assembly = typeof(Program).Assembly;

        static void Main(string[] args)
        {
            var streams = assembly.GetManifestResourceNames();

            BTTN4KNFEValues nfeValues = new BTTN4KNFEValues();
            nfeValues.kissType = BTTN4KType.Press.ToString();
            nfeValues.targetbodypart = BTTN4KBodyPart.Lips.ToString();
            nfeValues.actualbodypart = BTTN4KBodyPart.Lips.ToString();

            nfeValues.tod0approach = DateTime.UtcNow;
            Thread.Sleep(1000); // simulate the approach phase
            nfeValues.tod1press = DateTime.UtcNow;
            Thread.Sleep(1000); // simulate the press phase
            nfeValues.tod2sustain = DateTime.UtcNow;
            Thread.Sleep(1000); // simulate the sustain phase
            nfeValues.tod3release = DateTime.UtcNow;
            Thread.Sleep(1000); // simulate the release phase
            nfeValues.tod4recovery = DateTime.UtcNow;
            Thread.Sleep(1000); // simulate the recovery phase
            nfeValues.tod5finish = DateTime.UtcNow;
            BTTN4KNFEValues.ComputeDurations(nfeValues);

            string nfeJson = BTTN4KNFEFactory.FillTemplate(assembly, nfeValues);

            TrinityConfig.HttpPort = 8080 + 1;
#pragma warning disable CS0612 // Type or member is obsolete
            TrinityConfig.ServerPort = 5304 + 1;
#pragma warning restore CS0612 // Type or member is obsolete
            var agent = new NFELocalStorageAgentImplementation();
            agent.Start();

            using (var httpClient = new HttpClient())
            {
                string agentUrl = "http://localhost:8081/SendNFEEnvelopeToStorage/";
                Console.WriteLine(">>>Request:" + agentUrl);
                using (var requestMessage = new HttpRequestMessage(new HttpMethod("POST"), agentUrl))
                {
                    requestMessage.Headers.TryAddWithoutValidation("Accept", "application/json");
                    //string jsonRequest = JsonConvert.SerializeObject(request);
                    string jsonRequest = nfeJson;
                    requestMessage.Content = new StringContent(jsonRequest);
                    var task = httpClient.SendAsync(requestMessage);
                    task.Wait();
                    var result = task.Result;
                    string jsonResponse = result.Content.ReadAsStringAsync().Result;
                    Console.WriteLine(">>>Response:" + jsonResponse);
                    Console.WriteLine("Press Enter to continue...");
                    Console.ReadLine();
                }
            }

            nfeJson = File.ReadAllText("c:\\temp\\kissnfe4.json");
            using (var httpClient = new HttpClient())
            {
                string agentUrl = "http://localhost:8081/SendNFEEnvelopeToStorage/";
                Console.WriteLine(">>>Request:" + agentUrl);
                using (var requestMessage = new HttpRequestMessage(new HttpMethod("POST"), agentUrl))
                {
                    requestMessage.Headers.TryAddWithoutValidation("Accept", "application/json");
                    //string jsonRequest = JsonConvert.SerializeObject(request);
                    string jsonRequest = nfeJson;
                    requestMessage.Content = new StringContent(jsonRequest);
                    var task = httpClient.SendAsync(requestMessage);
                    task.Wait();
                    var result = task.Result;
                    string jsonResponse = result.Content.ReadAsStringAsync().Result;
                    Console.WriteLine(">>>Response:" + jsonResponse);
                    Console.WriteLine("Press Enter to continue...");
                    Console.ReadLine();
                }
            }

            byte[] res;
            int nBytes;
            using (var pngHappyFaceStream = assembly.GetManifestResourceStream("BTTN4KNFE.images.HappyFace.png"))
            {
                res = new byte[pngHappyFaceStream.Length];
                nBytes = pngHappyFaceStream.Read(res);
            }
            var happyFacepng64 = Convert.ToBase64String(res, 0, nBytes);

            DateTime tod0approach = DateTime.UtcNow; // always UTC
            DateTime tod1press = tod0approach.AddSeconds(3);
            DateTime tod2sustain = tod1press.AddSeconds(3);
            DateTime tod3release = tod2sustain.AddSeconds(5);
            DateTime tod4recovery = tod3release.AddSeconds(3);
            DateTime tod5finish = tod4recovery.AddSeconds(4);

            var nfeClaims = new BTTNFE_N4K_Claims();
            nfeClaims.InitializeTimeline(0, BTTN4KType.Press, BTTN4KMood.Savium, BTTN4KPurpose.Romantic,
                tod0approach, tod1press, tod2sustain, tod3release, tod4recovery, tod5finish);

            N4KSyntheticPressureCurve synthcurve = 
                new N4KSyntheticPressureCurve(nfeClaims.d1ms, nfeClaims.d2ms, nfeClaims.d3ms, nfeClaims.d4ms, nfeClaims.d5ms);
            //Array.Copy(synthcurve.Getd0approachCurve(), nfeClaims.d0approachcurve = new int[150], nfeClaims.d1s);
            //Array.Copy(synthcurve.Getd1pressCurve(), nfeClaims.d1presscurve = new int[150], nfeClaims.d2s);
            //Array.Copy(synthcurve.Getd2sustainCurve(), nfeClaims.d2sustaincurve = new int[150], nfeClaims.d3s);
            //Array.Copy(synthcurve.Getd3releaseCurve(), nfeClaims.d3releasecurve = new int[150], nfeClaims.d4s);
            //Array.Copy(synthcurve.Getd4recoveryCurve(), nfeClaims.d4recoverycurve = new int[150], nfeClaims.d5s);

            var apressure = synthcurve.Getd0approachCurve();
            nfeClaims.d0approachtime = new float[MAX_METRIC];
            nfeClaims.d0approachcurve = new float[MAX_METRIC];
            for (int t = 0; t < nfeClaims.d1s; t++)
            {
                nfeClaims.d0approachtime[t] = (float)(t * 0.1);
                nfeClaims.d0approachcurve[t] = (float)apressure[t];
            }

            var ppressure = synthcurve.Getd1pressCurve();
            nfeClaims.d1presstime = new float[MAX_METRIC];
            nfeClaims.d1presscurve = new float[MAX_METRIC];
            for (int t = 0; t < nfeClaims.d2s; t++)
            {
                nfeClaims.d1presstime[t] = (float)(t * 0.1);
                nfeClaims.d1presscurve[t] = (float)ppressure[t];
            }

            var spressure = synthcurve.Getd2sustainCurve();
            nfeClaims.d2sustaintime = new float[MAX_METRIC];
            nfeClaims.d2sustaincurve = new float[MAX_METRIC];
            for (int t = 0; t < nfeClaims.d3s; t++)
            {
                nfeClaims.d2sustaintime[t] = (float)(t * 0.1);
                nfeClaims.d2sustaincurve[t] = (float)spressure[t];
            }

            var rpressure = synthcurve.Getd3releaseCurve();
            nfeClaims.d3releasetime = new float[MAX_METRIC];
            nfeClaims.d3releasecurve = new float[MAX_METRIC];
            for (int t = 0; t < nfeClaims.d4s; t++)
            {
                nfeClaims.d3releasetime[t] = (float)(t * 0.1);
                nfeClaims.d3releasecurve[t] = (float)rpressure[t];
            }

            var vpressure = synthcurve.Getd4recoveryCurve();
            nfeClaims.d4recoverytime = new float[MAX_METRIC];
            nfeClaims.d4recoverycurve = new float[MAX_METRIC];
            for (int t = 0; t < nfeClaims.d5s; t++)
            {
                nfeClaims.d4recoverytime[t] = (float)(t * 0.1);
                nfeClaims.d4recoverycurve[t] = (float)vpressure[t];
            }

            int pressCoverage = 0; foreach (int pressure in ppressure) pressCoverage += pressure;
            int sustainCoverage = 0; foreach (int pressure in spressure) sustainCoverage += pressure;
            int releaseCoverage = 0; foreach (int pressure in rpressure) releaseCoverage += pressure;
            nfeClaims.coverage = pressCoverage + sustainCoverage + releaseCoverage;

            nfeClaims.d0approachpng64 = happyFacepng64;
            nfeClaims.d1presspng64 = happyFacepng64;
            nfeClaims.d2sustainpng64 = happyFacepng64;
            nfeClaims.d3releasepng64 = happyFacepng64;
            nfeClaims.d4recoverypng64 = happyFacepng64;

            //Console.WriteLine("nfeClaims:\n" + nfeClaims.ToString());

            string nfeUdid = "did:bluetoquenfe:" + Guid.NewGuid().ToString();
            string nfeCSUdid = nfeUdid;
            BTTNFE_N4K_EnvelopeContent nfeContent = new BTTNFE_N4K_EnvelopeContent(nfeUdid, BTTNFE_N4K_Claims.DefaultContext, nfeCSUdid, nfeClaims);

            BTTGenericCredential_PackingLabel nfeLabel = new BTTGenericCredential_PackingLabel(
                new List<string> { "Verifiable Credential", "Structured Credential", BTTGenericCredentialType.BlueToqueNfe.ToString() },
                BTTGenericCredentialType.BlueToqueNfe, 1, BTTTrustLevel.HashedThumbprint, BTTEncryptionFlag.NotEncrypted, 
                null, "N4K NFE Kiss", new List<string> { "An BTT N4K NFE Kiss Structured Credential" });
            BTTNFE_N4K_Envelope nfeEnvelope = new BTTNFE_N4K_Envelope(nfeUdid, nfeLabel, nfeContent);

            string envelope = nfeEnvelope.ToString();
            string nfeThumbprint64 = BTTN4KNFEFactoryHelpers.ComputeHash64(envelope);
            BTTGenericCredential_EnvelopeSeal nfeProof = new BTTGenericCredential_EnvelopeSeal(nfeThumbprint64, null, null, new List<string> { "SHA3-256 Hashed Thumbprint stored as Base64 string" });
            BTTNFE_N4K_SealedEnvelope nfeSealedEnvelope = new BTTNFE_N4K_SealedEnvelope(nfeEnvelope, nfeProof);

            string json1 = nfeSealedEnvelope.ToString();
            Console.WriteLine("nfeSealedEnvelope:\n" + json1);
            File.WriteAllText("c:\\temp\\nfeSealedEnvelope.json", json1);

            BTTNFE_N4K_SealedEnvelope_Cell nfeCell = new BTTNFE_N4K_SealedEnvelope_Cell(nfeSealedEnvelope);

            string json = nfeCell.ToString();
            Console.WriteLine("nfeCell:\n" + json);
            File.WriteAllText("c:\\temp\\nfeCell.json", json);

            Global.LocalStorage.SaveBTTNFE_N4K_SealedEnvelope_Cell(nfeCell);

            using (var httpClient = new HttpClient())
            {
                SendNFEEnvelopeRequest request = new SendNFEEnvelopeRequest(nfeSealedEnvelope);
                string agentUrl = "http://localhost:8081/SendNFEEnvelopeToStorage/";
                Console.WriteLine(">>>Request:" + agentUrl);
                using (var requestMessage = new HttpRequestMessage(new HttpMethod("POST"), agentUrl))
                {
                    requestMessage.Headers.TryAddWithoutValidation("Accept", "application/json");
                    //string jsonRequest = JsonConvert.SerializeObject(request);
                    string jsonRequest = request.ToString();
                    Console.WriteLine("jsonRequest:\n" + jsonRequest);
                    File.WriteAllText("c:\\temp\\jsonRequest.json", jsonRequest);
                    requestMessage.Content = new StringContent(jsonRequest);
                    var task = httpClient.SendAsync(requestMessage);
                    task.Wait();
                    var result = task.Result;
                    string jsonResponse = result.Content.ReadAsStringAsync().Result;
                    Console.WriteLine(">>>Response:" + jsonResponse);
                    Console.WriteLine("Press Enter to continue...");
                    Console.ReadLine();
                }
            }

            Console.WriteLine("Waiting...");
            Console.WriteLine("Press any key to exit ...");
            Console.ReadKey();

            agent.Stop();
            Global.Exit(0);
        }
    }
}

