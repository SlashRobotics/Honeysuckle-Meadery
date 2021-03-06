// Copyright © 2015 The CefSharp Authors. All rights reserved.
//
// Use of this source code is governed by a BSD-style license that can be found in the LICENSE file.

#pragma once

#include "Stdafx.h"
#include "CefWrapper.h"

namespace CefSharp
{
    namespace Internals
    {
        private ref class CefRequestCallbackWrapper : public IRequestCallback, public CefWrapper
        {
        private:
            MCefRefPtr<CefCallback> _callback;
            IFrame^ _frame;
            IRequest^ _request;

        internal:
            CefRequestCallbackWrapper(CefRefPtr<CefCallback> &callback)
                : _callback(callback), _frame(nullptr), _request(nullptr)
            {
            }

            CefRequestCallbackWrapper(
                CefRefPtr<CefCallback> &callback,
                IFrame^ frame,
                IRequest^ request)
                : _callback(callback), _frame(frame), _request(request)
            {
            }

            !CefRequestCallbackWrapper()
            {
                _callback = nullptr;
            }

            ~CefRequestCallbackWrapper()
            {
                this->!CefRequestCallbackWrapper();
                delete _request;
                _request = nullptr;
                delete _frame;
                _frame = nullptr;

                _disposed = true;
            }

        public:
            virtual void Continue(bool allow)
            {
                ThrowIfDisposed();

                if (allow)
                {
                    _callback->Continue();
                }
                else
                {
                    _callback->Cancel();
                }

                delete this;
            }

            virtual void Cancel()
            {
                ThrowIfDisposed();

                _callback->Cancel();

                delete this;
            }
        };
    }
}
