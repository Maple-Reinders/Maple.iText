/*
This file is part of the iText (R) project.
Copyright (c) 1998-2021 iText Group NV
Authors: iText Software.

This program is offered under a commercial and under the AGPL license.
For commercial licensing, contact us at https://itextpdf.com/sales.  For AGPL licensing, see below.

AGPL licensing:
This program is free software: you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU Affero General Public License for more details.

You should have received a copy of the GNU Affero General Public License
along with this program.  If not, see <https://www.gnu.org/licenses/>.
*/
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Common.Logging;
using iText.IO.Util;
using iText.Kernel;
using iText.Kernel.Actions.Events;
using iText.Kernel.Actions.Exceptions;
using iText.Kernel.Actions.Processors;
using iText.Kernel.Actions.Sequence;
using iText.Kernel.Counter.Context;

namespace iText.Kernel.Actions {
    /// <summary>Handles events based oh their origin.</summary>
    internal sealed class ProductEventHandler : AbstractContextBasedEventHandler {
        internal static readonly iText.Kernel.Actions.ProductEventHandler INSTANCE = new iText.Kernel.Actions.ProductEventHandler
            ();

        private static readonly ILog LOGGER = LogManager.GetLogger(typeof(iText.Kernel.Actions.ProductEventHandler
            ));

        private readonly ConcurrentDictionary<String, ITextProductEventProcessor> processors = new ConcurrentDictionary
            <String, ITextProductEventProcessor>();

        private readonly ConditionalWeakTable<SequenceId, IList<AbstractProductProcessITextEvent>> events = new ConditionalWeakTable
            <SequenceId, IList<AbstractProductProcessITextEvent>>();

        private ProductEventHandler()
            : base(UnknownContext.PERMISSIVE) {
        }

        /// <summary>
        /// Pass the event to the appropriate
        /// <see cref="iText.Kernel.Actions.Processors.ITextProductEventProcessor"/>.
        /// </summary>
        /// <param name="event">to handle</param>
        protected internal override void OnAcceptedEvent(AbstractContextBasedITextEvent @event) {
            if (!(@event is AbstractProductProcessITextEvent)) {
                return;
            }
            AbstractProductProcessITextEvent productEvent = (AbstractProductProcessITextEvent)@event;
            ITextProductEventProcessor productEventProcessor = FindProcessorForProduct(productEvent.GetProductName());
            productEventProcessor.OnEvent(productEvent);
            if (productEvent.GetSequenceId() != null) {
                if (productEvent is ConfirmEvent) {
                    WrapConfirmedEvent((ConfirmEvent)productEvent, productEventProcessor);
                }
                else {
                    AddEvent(productEvent.GetSequenceId(), productEvent);
                }
            }
        }

        internal ITextProductEventProcessor AddProcessor(ITextProductEventProcessor processor) {
            return processors.Put(processor.GetProductName(), processor);
        }

        internal ITextProductEventProcessor RemoveProcessor(String productName) {
            return processors.JRemove(productName);
        }

        internal ITextProductEventProcessor GetProcessor(String productName) {
            return processors.Get(productName);
        }

        internal IDictionary<String, ITextProductEventProcessor> GetProcessors() {
            return JavaCollectionsUtil.UnmodifiableMap(new Dictionary<String, ITextProductEventProcessor>(processors));
        }

        internal IList<AbstractProductProcessITextEvent> GetEvents(SequenceId id) {
            lock (events) {
                IList<AbstractProductProcessITextEvent> listOfEvents = events.Get(id);
                if (listOfEvents == null) {
                    return JavaCollectionsUtil.EmptyList<AbstractProductProcessITextEvent>();
                }
                return JavaCollectionsUtil.UnmodifiableList<AbstractProductProcessITextEvent>(new List<AbstractProductProcessITextEvent
                    >(listOfEvents));
            }
        }

        internal void AddEvent(SequenceId id, AbstractProductProcessITextEvent @event) {
            lock (events) {
                IList<AbstractProductProcessITextEvent> listOfEvents = events.Get(id);
                if (listOfEvents == null) {
                    listOfEvents = new List<AbstractProductProcessITextEvent>();
                    events.Put(id, listOfEvents);
                }
                listOfEvents.Add(@event);
            }
        }

        private void WrapConfirmedEvent(ConfirmEvent @event, ITextProductEventProcessor productEventProcessor) {
            lock (events) {
                IList<AbstractProductProcessITextEvent> eventsList = events.Get(@event.GetSequenceId());
                AbstractProductProcessITextEvent confirmedEvent = @event.GetConfirmedEvent();
                int indexOfReportedEvent = eventsList.IndexOf(confirmedEvent);
                if (indexOfReportedEvent >= 0) {
                    eventsList[indexOfReportedEvent] = new ConfirmedEventWrapper(confirmedEvent, productEventProcessor.GetUsageType
                        (), productEventProcessor.GetProducer());
                }
                else {
                    LOGGER.Warn(MessageFormatUtil.Format(KernelLogMessageConstant.UNREPORTED_EVENT, confirmedEvent.GetProductName
                        (), confirmedEvent.GetEventType()));
                }
            }
        }

        private ITextProductEventProcessor FindProcessorForProduct(String productName) {
            ITextProductEventProcessor processor = processors.Get(productName);
            if (processor != null) {
                return processor;
            }
            if (ProductNameConstant.PRODUCT_NAMES.Contains(productName)) {
                processor = new DefaultITextProductEventProcessor(productName);
                processors.Put(productName, processor);
                return processor;
            }
            else {
                throw new UnknownProductException(MessageFormatUtil.Format(UnknownProductException.UNKNOWN_PRODUCT, productName
                    ));
            }
        }
    }
}