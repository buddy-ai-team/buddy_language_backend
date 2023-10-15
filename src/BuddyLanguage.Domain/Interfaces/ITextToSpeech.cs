using BuddyLanguage.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuddyLanguage.Domain.Interfaces
{
    public interface ITextToSpeech
    {
        Task TextToMP3FileAsync(string text, string outputFilePath, SynthesisVoices voice);
    }

}
