﻿using System;

namespace ManagedBass.Effects
{
    public class PitchRecord
    {
        #region Constants
        static readonly double InverseLog2 = 1.0 / Math.Log10(2.0);

        public const int MinMidiNoteA0 = 21,
                         MaxMidiNoteC8 = 108;
        #endregion

        public PitchRecord(double Pitch)
        {
            var midiNote = 0;
            var midiCents = 0;

            PitchToMidiNote((float)Pitch, out midiNote, out midiCents);
            
            this.Pitch = Pitch;
            this.MidiNote = midiNote;
            this.MidiCents = midiCents;
            this.NoteName = GetNoteName(MidiNote, true, true);
        }

        /// <summary>
        /// The detected pitch
        /// </summary>
        public double Pitch { get; }

        /// <summary>
        /// The detected MIDI note, or 0 for no pitch
        /// </summary>
        public int MidiNote { get; }

        /// <summary>
        /// The offset from the detected MIDI note in cents, from -50 to +50.
        /// </summary>
        public int MidiCents { get; }

        public string NoteName { get; }
        
        /// <summary>
        /// Get the MIDI note and cents of the pitch 
        /// </summary>
        public static bool PitchToMidiNote(float pitch, out int note, out int cents)
        {
            if (pitch < 20)
            {
                note = cents = 0;
                return false;
            }

            var fNote = (float)((12.0 * Math.Log10(pitch / 55.0) * InverseLog2)) + 33.0f;
            note = (int)(fNote + 0.5f);
            cents = (int)((note - fNote) * 100);
            return true;
        }

        /// <summary>
        /// Get the pitch from the MIDI note
        /// </summary>
        public static float MidiNoteToPitch(float note)
        {
            if (note < 33) 
                return 0;

            return (float)Math.Pow(10.0, (note - 33.0f) / InverseLog2 / 12.0f) * 55.0f;
        }
        
        /// <summary>
        /// Format a midi note to text
        /// </summary>
        public static string GetNoteName(int note, bool sharps, bool showOctave)
        {
            if (note < MinMidiNoteA0 || note > MaxMidiNoteC8) return null;

            note -= MinMidiNoteA0;

            int octave = (note + 9) / 12;
            note = note % 12;
            string noteText = null;

            switch (note)
            {
                case 0:
                    noteText = "A";
                    break;

                case 1:
                    noteText = sharps ? "A#" : "Bb";
                    break;

                case 2:
                    noteText = "B";
                    break;

                case 3:
                    noteText = "C";
                    break;

                case 4:
                    noteText = sharps ? "C#" : "Db";
                    break;

                case 5:
                    noteText = "D";
                    break;

                case 6:
                    noteText = sharps ? "D#" : "Eb";
                    break;

                case 7:
                    noteText = "E";
                    break;

                case 8:
                    noteText = "F";
                    break;

                case 9:
                    noteText = sharps ? "F#" : "Gb";
                    break;

                case 10:
                    noteText = "G";
                    break;

                case 11:
                    noteText = sharps ? "G#" : "Ab";
                    break;
            }

            if (showOctave) noteText += " " + octave.ToString();

            return noteText;
        }
    }
}