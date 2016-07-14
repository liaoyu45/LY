using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace KeyboardSynchronizer {
    public class Yeller {
        public const int ENDLESS = -4;
        public const int FINISHED = -3;
        public const int TERMINALED = -2;
        public const int STARTED = -1;

        /// <summary>
        /// Yell 'FREE', can be terminated by <see cref="Keys.LControlKey"/>.
        /// </summary>
        public Yeller() : this(Keys.LControlKey, Keys.F, Keys.R, Keys.E, Keys.E) { }

        /// <summary>
        /// Yell pointed keys.
        /// </summary>
        /// <param name="shutupkey">When it is yelled, the <see cref="Yelling"/> event will be terminated or finished.</param>
        /// <param name="otherKeys"><see cref="Endless"/> will be true if it is empty.</param>
        public Yeller(Keys shutupkey, params Keys[] otherKeys) {
            ShutUpKey = shutupkey;
            if (otherKeys.Length == 0) {
                Endless = true;
                Length = int.MaxValue;
            } else {
                Content = otherKeys;
                foreach (var item in Content) {
                    if (shutupkey == item) {
                        throw new Exception(item.ToString());
                    }
                }
                Length = Content.Length;
                Text = string.Join(string.Empty, Content);
            }
        }

        public event Action<Yeller, YellEventArgs> Yelling;
        public Func<bool> CanYell;

        List<Keys> yelledContent = new List<Keys>();
        public bool IsYelling { get; private set; }
        /// <summary>
        /// The keys queue.
        /// </summary>
        public Keys[] Content { get; }
        /// <summary>
        /// If true, it will keep yelling unless <see cref="ShutUpKey"/> is yelled.
        /// </summary>
        public bool Endless { get; }
        /// <summary>
        /// Length of <see cref="Content"/>.
        /// </summary>
        public int Length { get; }
        public string Name { get; set; }
        /// <summary>
        /// Yell this to terminate or finish yelling.
        /// </summary>
        public Keys ShutUpKey { get; set; }
        /// <summary>
        /// Can be anything.
        /// </summary>
        public object Tag { get; set; }
        /// <summary>
        /// See <see cref="string.Join{T}(string, IEnumerable{T})"/>(<see cref="string.Empty"/>, <see cref="Content"/>).
        /// </summary>
        public string Text { get; }
        public static readonly Keys[] FREE = { Keys.F, Keys.R, Keys.E, Keys.E };

        /// <summary>
        /// <see cref="YellEventArgs.Index"/> of <see cref="Yelling"/> is <see cref="STARTED"/>.
        /// </summary>
        public void StartToYell() {
            if (CanYell?.Invoke() == false) {
                return;
            }
            if (!Endless) {
                if (Content == null || Content.Length == 0 || Yelling == null) {
                    return;
                }
            }
            if (IsYelling) {
                return;
            }
            IsYelling = true;
            yell(STARTED, default(Keys));
        }

        internal void Yell(Keys k) {
            if (!IsYelling) {
                return;
            }
            if (k == ShutUpKey) {
                finish(k);
            } else if (Endless) {
                yell(ENDLESS, k);
            } else {
                goOn(k);
            }
        }

        void yell(int i, Keys k) {
            try {
                Yelling(this, new YellEventArgs { Index = i, Key = k });
            } catch (Exception) {
            }
        }

        bool checkYelling() {
            return yelledContent.Count < Length;
        }

        void goOn(Keys k) {
            if (checkYelling()) {
                var current = Content[yelledContent.Count];
                if (current == default(Keys) || current == k) {
                    yell(yelledContent.Count, k);
                    yelledContent.Add(k);
                } else {
                    if (yelledContent.Count != 0) {
                        yelledContent.Clear();
                        goOn(k);
                    } else {
                        yell(STARTED, k);
                    }
                }
            } else {
                yelledContent.Clear();
                goOn(k);
            }
        }

        public void Restart() {
            yelledContent.Clear();
        }

        private void finish(Keys k) {
            if (checkYelling()) {
                yell(TERMINALED, k);
            } else {
                yell(FINISHED, k);
            }
            IsYelling = false;
            yelledContent.Clear();
        }

        public override string ToString() {
            return string.Join(string.Empty, Content);
        }
    }

    public class YellEventArgs : EventArgs {
        public int Index { get; set; }
        public Keys Key { get; set; }
    }
}
