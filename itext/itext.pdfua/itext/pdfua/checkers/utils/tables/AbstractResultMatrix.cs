/*
This file is part of the iText (R) project.
Copyright (c) 1998-2025 Apryse Group NV
Authors: Apryse Software.

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
using System.Collections.Generic;
using System.Text;
using iText.Commons.Utils;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Tagging;
using iText.Pdfua.Exceptions;

namespace iText.Pdfua.Checkers.Utils.Tables {
//\cond DO_NOT_DOCUMENT
    /// <summary>Class that represents a matrix of cells in a table.</summary>
    /// <remarks>
    /// Class that represents a matrix of cells in a table.
    /// It is used to check if the table has valid headers and scopes for the cells.
    /// </remarks>
    /// <typeparam name="T">the type of the cell</typeparam>
    internal abstract class AbstractResultMatrix<T> {
        protected internal readonly ITableIterator<T> iterator;

        // We can't use an array because it is not autoportable.
        private readonly IList<T> cellMatrix;

        private readonly int rows;

        private readonly int cols;

        private readonly PdfUAConformance conformance;

        /// <summary>
        /// Creates a new
        /// <see cref="AbstractResultMatrix{T}"/>
        /// instance.
        /// </summary>
        /// <param name="iterator">the iterator that will be used to iterate over the cells</param>
        /// <param name="conformance">
        /// 
        /// <see cref="iText.Kernel.Pdf.PdfUAConformance"/>
        /// of the document that is being checked
        /// </param>
        protected internal AbstractResultMatrix(ITableIterator<T> iterator, PdfUAConformance conformance) {
            this.conformance = conformance;
            this.rows = iterator.GetAmountOfRowsHeader() + iterator.GetAmountOfRowsBody() + iterator.GetAmountOfRowsFooter
                ();
            this.cols = iterator.GetNumberOfColumns();
            this.iterator = iterator;
            this.cellMatrix = iText.Pdfua.Checkers.Utils.Tables.AbstractResultMatrix<T>.CreateFixedSizedList<T>(rows *
                 cols, null);
        }

        /// <summary>Runs the algorithm to check if the table has valid headers and scopes for the cells.</summary>
        public virtual void CheckValidTableTagging() {
            ICollection<String> knownIds = new HashSet<String>();
            // We use boxed boolean array so we can don't duplicate our setCell methods.
            // But we fill default with false so we can avoid the null check.
            IList<bool> scopeMatrix = iText.Pdfua.Checkers.Utils.Tables.AbstractResultMatrix<T>.CreateFixedSizedList<bool
                >(rows * cols, false);
            bool hasUnknownHeaders = false;
            while (iterator.HasNext()) {
                T cell = iterator.Next();
                String role = GetRole(cell);
                int rowspan = iterator.GetRowspan();
                int colspan = iterator.GetColspan();
                int colIdx = iterator.GetCol();
                int rowIdx = iterator.GetRow();
                this.SetCell(rowIdx, rowspan, colIdx, colspan, cellMatrix, cell);
                if (StandardRoles.TH.Equals(role)) {
                    byte[] id = GetElementId(cell);
                    if (id != null) {
                        knownIds.Add(iText.Commons.Utils.JavaUtil.GetStringForBytes(id, System.Text.Encoding.UTF8));
                    }
                    String scope = GetScope(cell);
                    if (PdfName.Column.GetValue().Equals(scope)) {
                        this.SetColumnValue(colIdx, colspan, scopeMatrix, true);
                    }
                    else {
                        if (PdfName.Row.GetValue().Equals(scope)) {
                            this.SetRowValue(rowIdx, rowspan, scopeMatrix, true);
                        }
                        else {
                            if (PdfName.Both.GetValue().Equals(scope)) {
                                this.SetColumnValue(colIdx, colspan, scopeMatrix, true);
                                this.SetRowValue(rowIdx, rowspan, scopeMatrix, true);
                            }
                            else {
                                hasUnknownHeaders = true;
                                DetermineDefaultScope(rowIdx, rowspan, colIdx, colspan, scopeMatrix);
                            }
                        }
                    }
                }
                else {
                    if (!StandardRoles.TD.Equals(role)) {
                        String message = MessageFormatUtil.Format(PdfUAExceptionMessageConstants.CELL_HAS_INVALID_ROLE, GetNormalizedRow
                            (rowIdx), GetLocationInTable(rowIdx), colIdx);
                        throw new PdfUAConformanceException(message);
                    }
                }
            }
            ValidateTableCells(knownIds, scopeMatrix, hasUnknownHeaders);
        }

//\cond DO_NOT_DOCUMENT
        internal abstract IList<byte[]> GetHeaders(T cell);
//\endcond

//\cond DO_NOT_DOCUMENT
        internal abstract String GetScope(T cell);
//\endcond

//\cond DO_NOT_DOCUMENT
        internal abstract byte[] GetElementId(T cell);
//\endcond

//\cond DO_NOT_DOCUMENT
        internal abstract String GetRole(T cell);
//\endcond

        private void DetermineDefaultScope(int rowIdx, int rowspan, int colIdx, int colspan, IList<bool> scopeMatrix
            ) {
            if (conformance == PdfUAConformance.PDF_UA_1) {
                // Default values were introduced in PDF 2.0
                return;
            }
            // Assumed value for the Scope shall be determined (see Table 384 in ISO 32000-2:2020). These
            // assumptions are used by the algorithm for determining which headers are associated with a cell.
            // LrTb writing mode is taken into account, so default scope is applied to right/bottom cells.
            if ((rowIdx == 0 && colIdx == 0) || (rowIdx != 0 && colIdx != 0)) {
                SetRowAndColumnValue(rowIdx, this.rows - rowIdx, colIdx, colspan, scopeMatrix, true);
                SetRowAndColumnValue(rowIdx, rowspan, colIdx, this.cols - colIdx, scopeMatrix, true);
            }
            else {
                if (rowIdx == 0) {
                    SetRowAndColumnValue(rowIdx, this.rows - rowIdx, colIdx, colspan, scopeMatrix, true);
                }
                else {
                    SetRowAndColumnValue(rowIdx, rowspan, colIdx, this.cols - colIdx, scopeMatrix, true);
                }
            }
        }

        private void ValidateTableCells(ICollection<String> knownIds, IList<bool> scopeMatrix, bool hasUnknownHeaders
            ) {
            StringBuilder sb = new StringBuilder();
            bool areAllTDCellsValid = true;
            for (int i = 0; i < this.cellMatrix.Count; i++) {
                T cell = this.cellMatrix[i];
                if (cell == null) {
                    String message = MessageFormatUtil.Format(PdfUAExceptionMessageConstants.TABLE_CONTAINS_EMPTY_CELLS, GetNormalizedRow
                        (i), GetLocationInTable(i), i % this.cols);
                    throw new PdfUAConformanceException(message);
                }
                String role = GetRole(cell);
                if (!StandardRoles.TD.Equals(role)) {
                    continue;
                }
                if (HasValidHeaderIds(cell, knownIds)) {
                    continue;
                }
                bool hasConnectedHeader = (bool)scopeMatrix[i];
                if (!hasConnectedHeader && hasUnknownHeaders) {
                    // we don't want to break here, we want to collect all the errors
                    areAllTDCellsValid = false;
                    int row = i / this.cols;
                    int col = i % this.cols;
                    String location = GetLocationInTable(row);
                    String message = MessageFormatUtil.Format(PdfUAExceptionMessageConstants.CELL_CANT_BE_DETERMINED_ALGORITHMICALLY
                        , GetNormalizedRow(row), col, location);
                    sb.Append(message).Append('\n');
                }
            }
            if (!areAllTDCellsValid) {
                throw new PdfUAConformanceException(sb.ToString());
            }
        }

        private String GetLocationInTable(int row) {
            if (row < iterator.GetAmountOfRowsHeader()) {
                return "Header";
            }
            else {
                if (row < iterator.GetAmountOfRowsHeader() + iterator.GetAmountOfRowsBody()) {
                    return "Body";
                }
                else {
                    return "Footer";
                }
            }
        }

        private int GetNormalizedRow(int row) {
            if (row < iterator.GetAmountOfRowsHeader()) {
                return row;
            }
            else {
                if (row < iterator.GetAmountOfRowsHeader() + iterator.GetAmountOfRowsBody()) {
                    return row - iterator.GetAmountOfRowsHeader();
                }
                else {
                    return row - iterator.GetAmountOfRowsHeader() - iterator.GetAmountOfRowsBody();
                }
            }
        }

        private void SetCell<Z>(int row, int rowSpan, int col, int colSpan, IList<Z> arr, Z value) {
            for (int i = row; i < row + rowSpan; i++) {
                for (int j = col; j < col + colSpan; j++) {
                    arr[i * this.cols + j] = value;
                }
            }
        }

        private void SetRowValue(int row, int rowSpan, IList<bool> arr, bool value) {
            SetCell(row, rowSpan, 0, this.cols, arr, value);
        }

        private void SetColumnValue(int col, int colSpan, IList<bool> arr, bool value) {
            SetCell(0, this.rows, col, colSpan, arr, value);
        }

        private void SetRowAndColumnValue(int row, int rowSpan, int col, int colSpan, IList<bool> arr, bool value) {
            SetCell(row, rowSpan, col, colSpan, arr, value);
        }

        private bool HasValidHeaderIds(T cell, ICollection<String> knownIds) {
            IList<byte[]> headers = GetHeaders(cell);
            if (headers == null || headers.IsEmpty()) {
                return false;
            }
            foreach (byte[] knownId in headers) {
                if (!knownIds.Contains(iText.Commons.Utils.JavaUtil.GetStringForBytes(knownId, System.Text.Encoding.UTF8))
                    ) {
                    return false;
                }
            }
            return true;
        }

        private static IList<Z> CreateFixedSizedList<Z>(int capacity, Object defaultValue) {
            IList<Z> arr = new List<Z>(capacity);
            for (int i = 0; i < capacity; i++) {
                arr.Add((Z)defaultValue);
            }
            return arr;
        }
    }
//\endcond
}
