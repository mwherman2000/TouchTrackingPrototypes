using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;

namespace BTTN4KNFE
{
    /// <summary>
    /// Local extensions to the .NET runtime object representation of BTTNFE_N4K_Claims defined in TSL.
    /// </summary>
    public partial struct BTTNFE_N4K_Claims
    {
        public static readonly List<string> DefaultContext = new List<string> { "https://www.sovrona.com/ns/svrn/v1" };

        const int MILLI = 1000;

        public const int D1MSMAX = 5 * MILLI;
        public const int D2MSMAX = 10 * MILLI;
        public const int D3MSMAX = 10 * MILLI;
        public const int D4MSMAX = 10 * MILLI;
        public const int D5MSMAX = 5 * MILLI;

        public const int PEAKMIN = 0;
        public const int PEAKMAX = 9;

        public void InitializeTimeline(
            float kissCompass, BTTN4KType kissType, BTTN4KMood mood, BTTN4KPurpose purpose,
            DateTime tod0approach, DateTime tod1press, DateTime tod2sustain, DateTime tod3release, DateTime tod4recovery, DateTime tod5finish, 
            int peak = -1, BTTN4KBodyPart targetbodypart = BTTN4KBodyPart.Anywhere, BTTN4KBodyPart actualbodypart = BTTN4KBodyPart.Anywhere, bool tongue = false)
        {
            this.kissCompass = kissCompass;
            this.kissType = kissType;
            this.mood = mood;
            this.purpose = purpose;
            this.tongue = tongue;

            this.targetbodypart = targetbodypart;
            this.actualbodypart = actualbodypart;

            this.timezoneid = TimeZoneInfo.Utc.Id;
            this.tod0approach = tod0approach;
            this.tod1press = tod1press;
            this.tod2sustain = tod2sustain;
            this.tod3release = tod3release;
            this.tod4recovery = tod4recovery;
            this.tod5finish = tod5finish;

            this.t0ms = (int)Math.Round(tod0approach.Subtract(tod0approach).TotalMilliseconds); // zero
            this.t1ms = (int)Math.Round(tod1press.Subtract(tod0approach).TotalMilliseconds);
            this.t2ms = (int)Math.Round(tod2sustain.Subtract(tod0approach).TotalMilliseconds);
            this.t3ms = (int)Math.Round(tod3release.Subtract(tod0approach).TotalMilliseconds);
            this.t4ms = (int)Math.Round(tod4recovery.Subtract(tod0approach).TotalMilliseconds);
            this.t5ms = (int)Math.Round(tod5finish.Subtract(tod0approach).TotalMilliseconds);

            this.d1ms = t1ms - t0ms;
            this.d2ms = t2ms - t1ms;   
            this.d3ms = t3ms - t2ms;
            this.d4ms = t4ms - t3ms;
            this.d5ms = t5ms - t4ms;

            this.d1s = (int)Math.Round((double)this.d1ms / MILLI);
            this.d2s = (int)Math.Round((double)this.d2ms / MILLI);
            this.d3s = (int)Math.Round((double)this.d3ms / MILLI);
            this.d4s = (int)Math.Round((double)this.d4ms / MILLI);
            this.d5s = (int)Math.Round((double)this.d5ms / MILLI);

            this.t0s = 0;
            this.t1s = this.t0s + this.d1s;
            this.t2s = this.t1s + this.d2s;
            this.t3s = this.t2s + this.d3s;
            this.t4s = this.t3s + this.d4s;
            this.t5s = this.t4s = this.d5s;

            this.medianms = t2ms + (4 * d3ms - 2 * d2ms + 2 * d4ms) / 8;
            this.medians = t2s + (4 * d3s - 2 * d2s + 2 * d4s) / 8;

            this.peak = peak;
            if (this.peak == -1)
            {
                double ratio = (double)(this.d2ms + this.d3ms + this.d4ms) / (double)(D2MSMAX + D3MSMAX + D4MSMAX);
                this.peak = (int)Math.Round((double)PEAKMAX * 2.0 * ratio);
            }
            if (this.peak > PEAKMAX) this.peak = PEAKMAX;          
        }
    }
}
