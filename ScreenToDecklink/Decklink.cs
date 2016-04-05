using System;
using System.Windows.Forms;

using System.Runtime.InteropServices;
using DeckLinkAPI;
using System.Collections;

namespace ScreenToDecklink
{
    public class Decklink
    {
        private bool m_running;
        private ArrayList availableDeckLinks;
        private IDeckLinkOutput m_deckLinkOutput;

        private int m_frameWidth;
        private int m_frameHeight;
        private long m_frameDuration;
        private long m_frameTimescale;
        private uint m_framesPerSecond;
        private IDeckLinkMutableVideoFrame m_videoFrameBlack;
        private IDeckLinkMutableVideoFrame m_videoFrameBars;
        private uint m_totalFramesScheduled;

        public Decklink()
        {
            IDeckLinkIterator deckLinkIterator = new CDeckLinkIterator();
            if (deckLinkIterator == null)
            {
                throw new ApplicationException("This application requires the DeckLink drivers installed. Please install the Blackmagic DeckLink drivers to use the features of this application");
            }


            //m_running = false;

            //// Create the COM instance
            //IDeckLinkIterator deckLinkIterator = new CDeckLinkIterator();
            //if (deckLinkIterator == null)
            //{
            //    MessageBox.Show("This application requires the DeckLink drivers installed.\nPlease install the Blackmagic DeckLink drivers to use the features of this application", "Error");
            //    Environment.Exit(1);
            //}

            //// Get the first DeckLink card
            //deckLinkIterator.Next(out m_deckLink);
            //if (m_deckLink == null)
            //{
            //    MessageBox.Show("This application requires a DeckLink PCI card.\nYou will not be able to use the features of this application until a DeckLink PCI card is installed.", "Error");
            //    Environment.Exit(1);
            //}

            //// Get the IDeckLinkOutput interface
            //m_deckLinkOutput = (IDeckLinkOutput)m_deckLink;

            //// Provide this class as a delegate to the audio and video output interfaces
            //m_deckLinkOutput.SetScheduledFrameCompletionCallback(this);
	
            //// Populate the display mode combo with a list of display modes supported by the installed DeckLink card
            //IDeckLinkDisplayModeIterator displayModeIterator;

            ////comboBoxVideoFormat.BeginUpdate();
            ////comboBoxVideoFormat.Items.Clear();

            //m_deckLinkOutput.GetDisplayModeIterator(out displayModeIterator);

            //while (true)
            //{
            //    IDeckLinkDisplayMode deckLinkDisplayMode;

            //    displayModeIterator.Next(out deckLinkDisplayMode);
            //    if (deckLinkDisplayMode == null)
            //        break;

            //    //comboBoxVideoFormat.Items.Add(new DisplayModeEntry(deckLinkDisplayMode));
            //}

            //comboBoxVideoFormat.EndUpdate();
        }

        public ArrayList GetAvailableDeckLinks()
        {
            ArrayList list = new ArrayList();

            IDeckLinkIterator deckLinkIterator = new CDeckLinkIterator();
            if (deckLinkIterator == null)
            {
                throw new ApplicationException("This application requires the DeckLink drivers installed. Please install the Blackmagic DeckLink drivers to use the features of this application");
            }

            while (true)
            {
                IDeckLink deckLink;
                deckLinkIterator.Next(out deckLink);
                if (deckLink == null)
                    break;

                list.Add((IDeckLinkOutput) deckLink);
            }

            return list;
        }

        public ArrayList GetAvailableDisplayModes(IDeckLinkOutput deckLinkOutput)
        {
            ArrayList list = new ArrayList();

            IDeckLinkDisplayModeIterator displayModeIterator;
            deckLinkOutput.GetDisplayModeIterator(out displayModeIterator);
            while (true)
            {
                IDeckLinkDisplayMode deckLinkDisplayMode;

                displayModeIterator.Next(out deckLinkDisplayMode);
                if (deckLinkDisplayMode == null)
                    break;

                list.Add(deckLinkDisplayMode);
            }

            return list;
        }

        //private void StartRunning()
        //{
        //    //- Extract the IDeckLinkDisplayMode from the display mode popup menu
        //    IDeckLinkDisplayMode videoDisplayMode;
        //    videoDisplayMode = null; // ((DisplayModeEntry)comboBoxVideoFormat.SelectedItem).displayMode;
        //    m_frameWidth = videoDisplayMode.GetWidth();
        //    m_frameHeight = videoDisplayMode.GetHeight();
        //    videoDisplayMode.GetFrameRate(out m_frameDuration, out m_frameTimescale);
        //    // Calculate the number of frames per second, rounded up to the nearest integer.  For example, for NTSC (29.97 FPS), framesPerSecond == 30.
        //    m_framesPerSecond = (uint)((m_frameTimescale + (m_frameDuration-1))  /  m_frameDuration);

        //    // Set the video output mode
        //    m_deckLinkOutput.EnableVideoOutput(videoDisplayMode.GetDisplayMode(), _BMDVideoOutputFlags.bmdVideoOutputFlagDefault);

        //    // Generate a frame of black
        //    m_deckLinkOutput.CreateVideoFrame(m_frameWidth, m_frameHeight, m_frameWidth * 2, _BMDPixelFormat.bmdFormat8BitYUV, _BMDFrameFlags.bmdFrameFlagDefault, out m_videoFrameBlack);
        //    FillBlack(m_videoFrameBlack);

        //    // Generate a frame of colour bars
        //    m_deckLinkOutput.CreateVideoFrame(m_frameWidth, m_frameHeight, m_frameWidth * 2, _BMDPixelFormat.bmdFormat8BitYUV, _BMDFrameFlags.bmdFrameFlagDefault, out m_videoFrameBars);
        //    FillColourBars(m_videoFrameBars);

        //    // Begin video preroll by scheduling a second of frames in hardware
        //    m_totalFramesScheduled = 0;
        //    for (uint i = 0; i < m_framesPerSecond; i++)
        //        ScheduleNextFrame(true);

        //    m_running = true;
        //}

        //private void StopRunning()
        //{
        //    long unused;
        //    m_deckLinkOutput.StopScheduledPlayback(0, out unused, 100);
        //    m_deckLinkOutput.DisableVideoOutput();

        //    m_running = false;
        //}

        //private void ScheduleNextFrame(bool prerolling)
        //{
        //    if (prerolling == false)
        //    {
        //        // If not prerolling, make sure that playback is still active
        //        if (m_running == false)
        //            return;
        //    }

        //    IDeckLinkVideoFrame frame;

        //    if ((m_totalFramesScheduled % m_framesPerSecond) == 0)
        //    {
        //        // On each second, schedule a frame of bars
        //        m_deckLinkOutput.ScheduleVideoFrame(m_videoFrameBars, (m_totalFramesScheduled * m_frameDuration), m_frameDuration, m_frameTimescale);
        //    }
        //    else
        //    {
        //        // Schedue frames of black
        //        m_deckLinkOutput.ScheduleVideoFrame(m_videoFrameBlack, (m_totalFramesScheduled * m_frameDuration), m_frameDuration, m_frameTimescale);
        //    }

        //    m_totalFramesScheduled += 1;
        //}

        //// Explicit implementation of IDeckLinkVideoOutputCallback
        //void IDeckLinkVideoOutputCallback.ScheduledFrameCompleted(IDeckLinkVideoFrame completedFrame, _BMDOutputFrameCompletionResult result)
        //{
        //    // Note: if you throw an exception, it will be ignored by the caller.

        //    // When a frame has been completed
        //    ScheduleNextFrame(false);
        //}

        //void IDeckLinkVideoOutputCallback.ScheduledPlaybackHasStopped()
        //{
        //}

        //void FillColourBars(IDeckLinkVideoFrame theFrame)
        //{
        //    IntPtr          buffer;
        //    int             width, height;
        //    UInt32[]        bars = {0xEA80EA80, 0xD292D210, 0xA910A9A5, 0x90229035, 0x6ADD6ACA, 0x51EF515A, 0x286D28EF, 0x10801080};
        //    int             index = 0;

        //    theFrame.GetBytes(out buffer);
        //    width = theFrame.GetWidth();
        //    height = theFrame.GetHeight();

        //    for (uint y = 0; y < height; y++)
        //    {
        //        for (uint x = 0; x < width; x += 2)
        //        {
        //            // Write directly into unmanaged buffer
        //            Marshal.WriteInt32(buffer, index * 4, (Int32)bars[(x * 8) / width]);
        //            index++;
        //        }
        //    }
        //}

        //void FillBlack(IDeckLinkVideoFrame theFrame)
        //{
        //    IntPtr buffer;
        //    int             width, height;
        //    int             wordsRemaining;
        //    UInt32          black = 0x10801080;
        //    int             index = 0;

        //    theFrame.GetBytes(out buffer);
        //    width = theFrame.GetWidth();
        //    height = theFrame.GetHeight();

        //    wordsRemaining = (width * 2 * height) / 4;

        //    while (wordsRemaining-- > 0)
        //    {
        //        Marshal.WriteInt32(buffer, index*4, (Int32)black);
        //        index++;
        //    }
        //}

        ///// <summary>
        ///// Used for putting the IDeckLinkDisplayMode objects into the video format
        ///// combo box.
        ///// </summary>
        //struct DisplayModeEntry
        //{
        //    public IDeckLinkDisplayMode displayMode;

        //    public DisplayModeEntry(IDeckLinkDisplayMode displayMode)
        //    {
        //        this.displayMode = displayMode;
        //    }

        //    public override string ToString()
        //    {
        //        string str;

        //        displayMode.GetName(out str);

        //        return str;
        //    }
        //}

        ///// <summary>
        ///// Used for putting other object types into combo boxes.
        ///// </summary>
        //struct StringObjectPair<T>
        //{
        //    public string name;
        //    public T value;

        //    public StringObjectPair(string name, T value)
        //    {
        //        this.name = name;
        //        this.value = value;
        //    }

        //    public override string ToString()
        //    {
        //        return name;
        //    }
        //}
    }
}
