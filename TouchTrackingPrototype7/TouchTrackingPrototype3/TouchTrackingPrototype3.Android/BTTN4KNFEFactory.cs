using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace BTTN4KNFE
{
	public class BTTN4KNFEValues
	{
		public const int MAXSAMPLES = 150;
		public const int MILLI = 1000;

		public string udid;
		public string hashedThumbprint64;

		public float kissCompass;   // degrees - 0 degrees = UP
		public string purpose;      // "romantic"
		public string kissType;
		public string mood;         // "Savium"
		public bool tongue;         // false

		public string targetbodypart;
		public string actualbodypart;

		public string timezoneid;   // "UTC"
		public DateTime tod0approach;
		public DateTime tod1press;
		public DateTime tod2sustain;
		public DateTime tod3release;
		public DateTime tod4recovery;
		public DateTime tod5finish;

		public int t0ms;            // = 0
		public int t1ms;
		public int t2ms;
		public int t3ms;
		public int t4ms;
		public int t5ms;

		public int d1ms;
		public int d2ms;
		public int d3ms;
		public int d4ms;
		public int d5ms;

		public int d1s;
		public int d2s;
		public int d3s;
		public int d4s;
		public int d5s;

		public int t0s;             // = 0
		public int t1s;
		public int t2s;
		public int t3s;
		public int t4s;
		public int t5s;

		public float peak;
		public float medianms;
		public float medians;
		public float coverage;

		public int n1samples;
		public int n2samples;
		public int n3samples;
		public int n4samples;
		public int n5samples;

		public float[] d1approachtime;
		public float[] d2presstime;
		public float[] d3sustaintime;
		public float[] d4releasetime;
		public float[] d5recoverytime;

		public float[] d1approachcurve;
		public float[] d2presscurve;
		public float[] d3sustaincurve;
		public float[] d4releasecurve;
		public float[] d5recoverycurve;

		//public string d1approachpng64;
		//public string d2presspng64;
		//public string d3sustainpng64;
		//public string d4releasepng64;
		//public string d5recoverypng64;

		public BTTN4KNFEValues()
		{
			udid = Guid.NewGuid().ToString();

			kissCompass = 0;
			purpose = "romantic";
			mood = "Savium";
			tongue = false;
			timezoneid = "UTC";
			kissType = "Press";
			targetbodypart = "Lips";
			actualbodypart = "Lips";

			tod0approach = DateTime.MinValue;
			tod1press = DateTime.MinValue;
			tod2sustain = DateTime.MinValue;
			tod3release = DateTime.MinValue;
			tod4recovery = DateTime.MinValue;
			tod5finish = DateTime.MinValue;

			n1samples = 1;
			n2samples = 1;
			n3samples = 1;
			n4samples = 1;
			n5samples = 1;

			d1approachcurve = new float[MAXSAMPLES];
			d1approachtime = new float[MAXSAMPLES];
			d2presscurve = new float[MAXSAMPLES];
			d2presstime = new float[MAXSAMPLES];
			d3sustaincurve = new float[MAXSAMPLES];
			d3sustaintime = new float[MAXSAMPLES];
			d4releasecurve = new float[MAXSAMPLES];
			d4releasetime = new float[MAXSAMPLES];
			d5recoverycurve = new float[MAXSAMPLES];
			d5recoverytime = new float[MAXSAMPLES];
		}

		public static string ConvertFloatArrayToString(float[] array)
		{
			string values = "";
			int len = array.Length;
			for (int i = 0; i < len; i++) values += array[i].ToString() + ", ";
			values = values.Substring(0, values.Length - 2);
			return values;
		}

		public static void ComputeDurations(BTTN4KNFEValues nfeValues)
		{
			nfeValues.t0ms = (int)Math.Round(nfeValues.tod0approach.Subtract(nfeValues.tod0approach).TotalMilliseconds); // zero
			nfeValues.t1ms = (int)Math.Round(nfeValues.tod1press.Subtract(nfeValues.tod0approach).TotalMilliseconds);
			nfeValues.t2ms = (int)Math.Round(nfeValues.tod2sustain.Subtract(nfeValues.tod0approach).TotalMilliseconds);
			nfeValues.t3ms = (int)Math.Round(nfeValues.tod3release.Subtract(nfeValues.tod0approach).TotalMilliseconds);
			nfeValues.t4ms = (int)Math.Round(nfeValues.tod4recovery.Subtract(nfeValues.tod0approach).TotalMilliseconds);
			nfeValues.t5ms = (int)Math.Round(nfeValues.tod5finish.Subtract(nfeValues.tod0approach).TotalMilliseconds);

			nfeValues.d1ms = nfeValues.t1ms - nfeValues.t0ms;
			nfeValues.d2ms = nfeValues.t2ms - nfeValues.t1ms;
			nfeValues.d3ms = nfeValues.t3ms - nfeValues.t2ms;
			nfeValues.d4ms = nfeValues.t4ms - nfeValues.t3ms;
			nfeValues.d5ms = nfeValues.t5ms - nfeValues.t4ms;

			nfeValues.d1s = (int)Math.Round((double)nfeValues.d1ms / MILLI);
			nfeValues.d2s = (int)Math.Round((double)nfeValues.d2ms / MILLI);
			nfeValues.d3s = (int)Math.Round((double)nfeValues.d3ms / MILLI);
			nfeValues.d4s = (int)Math.Round((double)nfeValues.d4ms / MILLI);
			nfeValues.d5s = (int)Math.Round((double)nfeValues.d5ms / MILLI);

			nfeValues.t0s = 0;
			nfeValues.t1s = nfeValues.t0s + nfeValues.d1s;
			nfeValues.t2s = nfeValues.t1s + nfeValues.d2s;
			nfeValues.t3s = nfeValues.t2s + nfeValues.d3s;
			nfeValues.t4s = nfeValues.t3s + nfeValues.d4s;
			nfeValues.t5s = nfeValues.t4s = nfeValues.d5s;
			nfeValues.medians = nfeValues.t2s + (4 * nfeValues.d3s - 2 * nfeValues.d2s + 2 * nfeValues.d4s) / 8;

			nfeValues.t0ms = 0;
			nfeValues.t1ms = nfeValues.t0ms + nfeValues.d1ms;
			nfeValues.t2ms = nfeValues.t1ms + nfeValues.d2ms;
			nfeValues.t3ms = nfeValues.t2ms + nfeValues.d3ms;
			nfeValues.t4ms = nfeValues.t3ms + nfeValues.d4ms;
			nfeValues.t5ms = nfeValues.t4ms + nfeValues.d5ms;
			nfeValues.medianms = nfeValues.t2ms + (4 * nfeValues.d3ms - 2 * nfeValues.d2ms + 2 * nfeValues.d4ms) / 8;

			nfeValues.peak = ComputePeak(nfeValues);
			nfeValues.coverage = ComputeCoverage(nfeValues);
		}

		public static float FloatArrayPeak(int nsamples, float[] array)
		{
			float peak = 0;
			if (nsamples > array.Length) nsamples = array.Length;
			for (int i = 0; i < nsamples; i++) if (array[i] > peak) peak = array[i];
			return peak;
		}

		public static float FloatArrayCrossProduct(int nsamples, float[] array1, float[] array2)
		{
			float product = 0;
			if (nsamples > array1.Length) nsamples = array1.Length;
			if (nsamples > array2.Length) nsamples = array2.Length;
			for (int i = 0; i < nsamples; i++) product += array1[i] * array2[i];
			return product;
		}

		public static float ComputePeak(BTTN4KNFEValues nfeValues)
		{
			float peak = 0;

			peak = FloatArrayPeak(nfeValues.n3samples, nfeValues.d3sustaincurve);

			return peak;
		}

		public static float ComputeCoverage(BTTN4KNFEValues nfeValues)
		{
			float coverage = 0;

			coverage += FloatArrayCrossProduct(nfeValues.n2samples, nfeValues.d2presscurve, nfeValues.d2presstime);
			coverage += FloatArrayCrossProduct(nfeValues.n3samples, nfeValues.d3sustaincurve, nfeValues.d3sustaintime);
			coverage += FloatArrayCrossProduct(nfeValues.n4samples, nfeValues.d4releasecurve, nfeValues.d4releasetime);

			return coverage;
		}
	}

	public class BTTN4KNFEFactory
	{
		static string nfeJsonEnvelopeTemplate = null;
		static string nfeJsonProofTemplate = null;
		public static string nfePath;

		public static string FillTemplate(TouchTrackingPrototype3.Droid.MainActivity activity, BTTN4KNFEValues nfeValues)
		{
			string nfeJson = "";

#if false // Windows
			var streams = assembly.GetManifestResourceNames();
			var nfeJsonEnvelopeStream = assembly.GetManifestResourceStream("BTTN4KNFE.BTTN4KNFEFactoryEnvelope4.json");
			byte[] res = new byte[nfeJsonEnvelopeStream.Length];
			int nBytes = nfeJsonEnvelopeStream.Read(res);
			string nfeJsonEnvelopeTemplate = Encoding.UTF8.GetString(res);

			var nfeJsonProofStream = assembly.GetManifestResourceStream("BTTN4KNFE.BTTN4KNFEFactoryProof4.json");
			res = new byte[nfeJsonProofStream.Length];
			nBytes = nfeJsonProofStream.Read(res);
			string nfeJsonProofTemplate = Encoding.UTF8.GetString(res);
#else // Android
			if (String.IsNullOrEmpty(nfeJsonEnvelopeTemplate))
			{
				int proof4 = TouchTrackingPrototype3.Droid.Resource.Raw.proof4;
				int envelope4 = TouchTrackingPrototype3.Droid.Resource.Raw.envelope4;

				nfeJsonProofTemplate = BTTN4KNFEFactoryHelpers.LoadJsonTemplate(activity, proof4);
				nfeJsonEnvelopeTemplate = BTTN4KNFEFactoryHelpers.LoadJsonTemplate(activity, envelope4);
			}
#endif

			BTTN4KNFEValues.ComputeDurations(nfeValues);

			string nfeJsonEnvelope = nfeJsonEnvelopeTemplate;
			nfeJsonEnvelope = nfeJsonEnvelope.Replace("%udid%", nfeValues.udid);
			nfeJsonEnvelope = nfeJsonEnvelope.Replace("%kissCompass%", nfeValues.kissCompass.ToString());
			nfeJsonEnvelope = nfeJsonEnvelope.Replace("%purpose%", nfeValues.purpose.ToString());
			nfeJsonEnvelope = nfeJsonEnvelope.Replace("%mood%", nfeValues.mood.ToString());
			nfeJsonEnvelope = nfeJsonEnvelope.Replace("%tongue%", nfeValues.tongue.ToString());
			nfeJsonEnvelope = nfeJsonEnvelope.Replace("%timezoneid%", nfeValues.timezoneid.ToString());

			nfeJsonEnvelope = nfeJsonEnvelope.Replace("%kissType%", nfeValues.kissType.ToString());
			nfeJsonEnvelope = nfeJsonEnvelope.Replace("%targetbodypart%", nfeValues.targetbodypart.ToString());
			nfeJsonEnvelope = nfeJsonEnvelope.Replace("%actualbodypart%", nfeValues.actualbodypart.ToString());

			nfeJsonEnvelope = nfeJsonEnvelope.Replace("%tod0approach%", nfeValues.tod0approach.ToString());
			nfeJsonEnvelope = nfeJsonEnvelope.Replace("%tod1press%", nfeValues.tod1press.ToString());
			nfeJsonEnvelope = nfeJsonEnvelope.Replace("%tod2sustain%", nfeValues.tod2sustain.ToString());
			nfeJsonEnvelope = nfeJsonEnvelope.Replace("%tod3release%", nfeValues.tod3release.ToString());
			nfeJsonEnvelope = nfeJsonEnvelope.Replace("%tod4recovery%", nfeValues.tod4recovery.ToString());
			nfeJsonEnvelope = nfeJsonEnvelope.Replace("%tod5finish%", nfeValues.tod5finish.ToString());

			nfeJsonEnvelope = nfeJsonEnvelope.Replace("%t0ms%", nfeValues.t0ms.ToString());
			nfeJsonEnvelope = nfeJsonEnvelope.Replace("%t1ms%", nfeValues.t1ms.ToString());
			nfeJsonEnvelope = nfeJsonEnvelope.Replace("%t2ms%", nfeValues.t2ms.ToString());
			nfeJsonEnvelope = nfeJsonEnvelope.Replace("%t3ms%", nfeValues.t3ms.ToString());
			nfeJsonEnvelope = nfeJsonEnvelope.Replace("%t4ms%", nfeValues.t4ms.ToString());
			nfeJsonEnvelope = nfeJsonEnvelope.Replace("%t5ms%", nfeValues.t5ms.ToString());

			nfeJsonEnvelope = nfeJsonEnvelope.Replace("%d1ms%", nfeValues.d1ms.ToString());
			nfeJsonEnvelope = nfeJsonEnvelope.Replace("%d2ms%", nfeValues.d2ms.ToString());
			nfeJsonEnvelope = nfeJsonEnvelope.Replace("%d3ms%", nfeValues.d3ms.ToString());
			nfeJsonEnvelope = nfeJsonEnvelope.Replace("%d4ms%", nfeValues.d4ms.ToString());
			nfeJsonEnvelope = nfeJsonEnvelope.Replace("%d5ms%", nfeValues.d5ms.ToString());

			nfeJsonEnvelope = nfeJsonEnvelope.Replace("%d1s%", nfeValues.d1s.ToString());
			nfeJsonEnvelope = nfeJsonEnvelope.Replace("%d2s%", nfeValues.d2s.ToString());
			nfeJsonEnvelope = nfeJsonEnvelope.Replace("%d3s%", nfeValues.d3s.ToString());
			nfeJsonEnvelope = nfeJsonEnvelope.Replace("%d4s%", nfeValues.d4s.ToString());
			nfeJsonEnvelope = nfeJsonEnvelope.Replace("%d5s%", nfeValues.d5s.ToString());

			nfeJsonEnvelope = nfeJsonEnvelope.Replace("%t0s%", nfeValues.t0s.ToString());
			nfeJsonEnvelope = nfeJsonEnvelope.Replace("%t1s%", nfeValues.t1s.ToString());
			nfeJsonEnvelope = nfeJsonEnvelope.Replace("%t2s%", nfeValues.t2s.ToString());
			nfeJsonEnvelope = nfeJsonEnvelope.Replace("%t3s%", nfeValues.t3s.ToString());
			nfeJsonEnvelope = nfeJsonEnvelope.Replace("%t4s%", nfeValues.t4s.ToString());
			nfeJsonEnvelope = nfeJsonEnvelope.Replace("%t5s%", nfeValues.t5s.ToString());

			nfeJsonEnvelope = nfeJsonEnvelope.Replace("%peak%", nfeValues.peak.ToString());
			nfeJsonEnvelope = nfeJsonEnvelope.Replace("%medianms%", nfeValues.medianms.ToString());
			nfeJsonEnvelope = nfeJsonEnvelope.Replace("%medians%", nfeValues.medians.ToString());
			nfeJsonEnvelope = nfeJsonEnvelope.Replace("%coverage%", nfeValues.coverage.ToString());

			nfeJsonEnvelope = nfeJsonEnvelope.Replace("%n1samples%", nfeValues.n1samples.ToString());
			nfeJsonEnvelope = nfeJsonEnvelope.Replace("%n2samples%", nfeValues.n2samples.ToString());
			nfeJsonEnvelope = nfeJsonEnvelope.Replace("%n3samples%", nfeValues.n3samples.ToString());
			nfeJsonEnvelope = nfeJsonEnvelope.Replace("%n4samples%", nfeValues.n4samples.ToString());
			nfeJsonEnvelope = nfeJsonEnvelope.Replace("%n5samples%", nfeValues.n5samples.ToString());

			nfeJsonEnvelope = nfeJsonEnvelope.Replace("\"%d1approachcurve%\"", BTTN4KNFEValues.ConvertFloatArrayToString(nfeValues.d1approachcurve));
			nfeJsonEnvelope = nfeJsonEnvelope.Replace("\"%d1approachtime%\"", BTTN4KNFEValues.ConvertFloatArrayToString(nfeValues.d1approachtime));
			nfeJsonEnvelope = nfeJsonEnvelope.Replace("\"%d2presscurve%\"", BTTN4KNFEValues.ConvertFloatArrayToString(nfeValues.d2presscurve));
			nfeJsonEnvelope = nfeJsonEnvelope.Replace("\"%d2presstime%\"", BTTN4KNFEValues.ConvertFloatArrayToString(nfeValues.d2presstime));
			nfeJsonEnvelope = nfeJsonEnvelope.Replace("\"%d3sustaincurve%\"", BTTN4KNFEValues.ConvertFloatArrayToString(nfeValues.d3sustaincurve));
			nfeJsonEnvelope = nfeJsonEnvelope.Replace("\"%d3sustaintime%\"", BTTN4KNFEValues.ConvertFloatArrayToString(nfeValues.d3sustaintime));
			nfeJsonEnvelope = nfeJsonEnvelope.Replace("\"%d4releasecurve%\"", BTTN4KNFEValues.ConvertFloatArrayToString(nfeValues.d4releasecurve));
			nfeJsonEnvelope = nfeJsonEnvelope.Replace("\"%d4releasetime%\"", BTTN4KNFEValues.ConvertFloatArrayToString(nfeValues.d4releasetime));
			nfeJsonEnvelope = nfeJsonEnvelope.Replace("\"%d5recoverycurve%\"", BTTN4KNFEValues.ConvertFloatArrayToString(nfeValues.d5recoverycurve));
			nfeJsonEnvelope = nfeJsonEnvelope.Replace("\"%d5recoverytime%\"", BTTN4KNFEValues.ConvertFloatArrayToString(nfeValues.d5recoverytime));

			nfeValues.hashedThumbprint64 = BTTN4KNFEFactoryHelpers.ComputeHash64(nfeJsonEnvelope);
			dynamic nfeJsonCanconical = JsonConvert.DeserializeObject(nfeJsonEnvelope);
			string nfeJsonCanoncialString = JsonConvert.SerializeObject((object)nfeJsonCanconical);
			nfeValues.hashedThumbprint64 = BTTN4KNFEFactoryHelpers.ComputeHash64(nfeJsonCanoncialString);

			string nfeJsonProof = nfeJsonProofTemplate;
			nfeJsonProof = nfeJsonProof.Replace("\"%envelope%\"", nfeJsonCanoncialString);
			nfeJsonProof = nfeJsonProof.Replace("%hashedThumbprint64%", nfeValues.hashedThumbprint64);

			nfeJson = nfeJsonProof;

			return nfeJson;
		}

		public static async void CheckPermissions()
        {
			// TODO - do a check before the request
			var statusw = await Xamarin.Essentials.Permissions.RequestAsync<Xamarin.Essentials.Permissions.StorageWrite>();
			var statusr = await Xamarin.Essentials.Permissions.RequestAsync<Xamarin.Essentials.Permissions.StorageRead>();
		}

		public static void SaveNfe(string filename, string nfeJson)
		{
			CheckPermissions();

			System.Diagnostics.Debug.WriteLine(Android.OS.Environment.ExternalStorageDirectory.AbsolutePath);
			System.Diagnostics.Debug.WriteLine(Android.OS.Environment.DirectoryDownloads);
			string localFolder = Path.Combine(Android.OS.Environment.ExternalStorageDirectory.AbsolutePath, Android.OS.Environment.DirectoryDownloads);
			nfePath = Path.Combine(localFolder, filename);
			System.Diagnostics.Debug.WriteLine(nfePath);

			File.WriteAllText(nfePath, nfeJson);
		}
	}
}
