using System;
using System.Collections.Generic;
using NilanToolkit.SingletonHelper;
using UnityEngine;

namespace NilanToolkit.Ticking {
    public class TickCore : SingletonBehaviour<TickCore> {
        
        
        private readonly List<Ticker> _tickers = new List<Ticker>();
        
        private readonly Queue<Ticker> _addBuffer = new Queue<Ticker>();
        
        private readonly Queue<Ticker> _removeBuffer = new Queue<Ticker>();
        
        public void RegisterTicker(Ticker ticker) {
            _addBuffer.Enqueue(ticker);
        }

        public void UnRegisterTicker(Ticker ticker) {
            _removeBuffer.Enqueue(ticker);
        }
        
        public void Pause(bool enable) {
            for (var index = 0; index < _tickers.Count; index++) {
                var ticker = _tickers[index];
                ticker.Pause = enable;
            }
        }

        private void _TickAll() {
            lock (_tickers) {
                for (var i = 0; i < _tickers.Count; i++) {
                    var ticker = _tickers[i];
                    if (ticker == null) {
                        UnRegisterTicker(ticker);
                        continue;
                    }

                    if (ticker.IsFinish) {
                        ticker.CallTickEnd();
                        UnRegisterTicker(ticker);
                        continue;
                    }

                    if (!ticker.Active) {
                        ticker.CallTickStart();
                    }

                    ticker.Tick(Time.deltaTime);
                }

                while (_removeBuffer.Count > 0) {
                    _tickers.Remove(_removeBuffer.Dequeue());
                }

                while (_addBuffer.Count > 0) {
                    _tickers.Add(_addBuffer.Dequeue());
                }
            }
        }

        private void Update () {
            _TickAll();
        }

    }
}
