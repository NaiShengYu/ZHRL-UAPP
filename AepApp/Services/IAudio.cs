using System;

namespace SimpleAudioForms
{
	public interface IAudio
	{
        void PlayNetFile(string fileName);
        void PlayLocalFile(string fileName);
        void stopPlay();
	}
}