using DeckLinkAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ScreenToDecklink
{
    class DeckLinkRenderer : IDeckLinkVideoOutputCallback, IDeckLinkAudioOutputCallback, IDeckLinkScreenPreviewCallback
    {
        private bool running;
        private IDeckLink deckLink;
        private IDeckLinkOutput deckLinkOutput;
        private IDeckLinkDisplayMode displayMode;

        private int frameWidth;
        private int frameHeight;
        private long frameDuration;
        private long frameTimescale;
        private uint framesPerSecond;
        private IDeckLinkMutableVideoFrame videoFrameBlack;
        private IDeckLinkMutableVideoFrame videoFrameBars;
        private uint totalFramesScheduled;

        public DeckLinkRenderer(IDeckLinkOutput newDeckLinkOutput, IDeckLinkDisplayMode newDisplayMode)
        {
            running = false;
            deckLinkOutput = newDeckLinkOutput;
            displayMode = newDisplayMode;

            frameWidth = displayMode.GetWidth();
            frameHeight = displayMode.GetHeight();

            displayMode.GetFrameRate(out frameDuration, out frameTimescale);
            framesPerSecond = (uint)((frameTimescale + (frameDuration - 1)) / frameDuration);

            deckLinkOutput.SetScreenPreviewCallback(this);
            deckLinkOutput.SetScheduledFrameCompletionCallback(this);
            deckLinkOutput.SetAudioCallback(this);
        }

        public void Start()
        {
            uint audioChannelCount = 2;
            _BMDAudioSampleType audioSampleDepth = _BMDAudioSampleType.bmdAudioSampleType16bitInteger;
            _BMDAudioSampleRate audioSampleRate = _BMDAudioSampleRate.bmdAudioSampleRate48kHz;

            deckLinkOutput.EnableVideoOutput(displayMode.GetDisplayMode(), _BMDVideoOutputFlags.bmdVideoOutputFlagDefault);
            deckLinkOutput.EnableAudioOutput(audioSampleRate, audioSampleDepth, audioChannelCount, _BMDAudioOutputStreamType.bmdAudioOutputStreamContinuous);

            deckLinkOutput.CreateVideoFrame(frameWidth, frameHeight, frameWidth * 2, _BMDPixelFormat.bmdFormat8BitYUV, _BMDFrameFlags.bmdFrameFlagDefault, out videoFrameBlack);
            FillBlack(videoFrameBlack);

            deckLinkOutput.CreateVideoFrame(frameWidth, frameHeight, frameWidth * 2, _BMDPixelFormat.bmdFormat8BitYUV, _BMDFrameFlags.bmdFrameFlagDefault, out videoFrameBars);
            FillColourBars(videoFrameBars);

            totalFramesScheduled = 0;
            for (uint i = 0; i < framesPerSecond; i++)
            {
                ScheduleNextFrame(true);
            }

            deckLinkOutput.BeginAudioPreroll();

            running = true;
        }

        public void Stop()
        {
            long unused;
            deckLinkOutput.StopScheduledPlayback(0, out unused, 100);
            deckLinkOutput.DisableVideoOutput();

            running = false;
        }

        public void ScheduledFrameCompleted(IDeckLinkVideoFrame completedFrame, _BMDOutputFrameCompletionResult result)
        {
            System.Diagnostics.Debug.WriteLine("ScheduledFrameCompleted: " + result);
            ScheduleNextFrame(false);
        }

        public void ScheduledPlaybackHasStopped()
        {
            System.Diagnostics.Debug.WriteLine("ScheduledPlaybackHasStopped");
        }

        public void RenderAudioSamples(int preroll)
        {
            System.Diagnostics.Debug.WriteLine("RenderAudioSamples");

            if (preroll != 0)
            {
                System.Diagnostics.Debug.WriteLine("Starting ScheduledPlayback...");
                deckLinkOutput.StartScheduledPlayback(0, 100, 1.0);
            }
        }

        public void DrawFrame(IDeckLinkVideoFrame theFrame)
        {
            System.Diagnostics.Debug.WriteLine("theFrame");
        }

        private void ScheduleNextFrame(bool prerolling)
        {
            if (prerolling == false)
            {
                // If not prerolling, make sure that playback is still active
                if (running == false)
                    return;
            }

            if ((totalFramesScheduled % framesPerSecond) == 0)
            {
                // On each second, schedule a frame of bars
                deckLinkOutput.ScheduleVideoFrame(videoFrameBars, (totalFramesScheduled * frameDuration), frameDuration, frameTimescale);
            }
            else
            {
                // Schedue frames of black
                deckLinkOutput.ScheduleVideoFrame(videoFrameBlack, (totalFramesScheduled * frameDuration), frameDuration, frameTimescale);
            }

            totalFramesScheduled += 1;
        }

        private void FillColourBars(IDeckLinkVideoFrame theFrame)
        {
            IntPtr buffer;
            int width, height;
            UInt32[] bars = { 0xEA80EA80, 0xD292D210, 0xA910A9A5, 0x90229035, 0x6ADD6ACA, 0x51EF515A, 0x286D28EF, 0x10801080 };
            int index = 0;

            theFrame.GetBytes(out buffer);
            width = theFrame.GetWidth();
            height = theFrame.GetHeight();

            for (uint y = 0; y < height; y++)
            {
                for (uint x = 0; x < width; x += 2)
                {
                    // Write directly into unmanaged buffer
                    Marshal.WriteInt32(buffer, index * 4, (Int32)bars[(x * 8) / width]);
                    index++;
                }
            }
        }

        private void FillBlack(IDeckLinkVideoFrame theFrame)
        {
            IntPtr buffer;
            int width, height;
            int wordsRemaining;
            UInt32 black = 0x10801080;
            int index = 0;

            theFrame.GetBytes(out buffer);
            width = theFrame.GetWidth();
            height = theFrame.GetHeight();

            wordsRemaining = (width * 2 * height) / 4;

            while (wordsRemaining-- > 0)
            {
                Marshal.WriteInt32(buffer, index * 4, (Int32)black);
                index++;
            }
        }
    }
}
