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
using iText.Test;

namespace iText.Kernel.Geom {
    [NUnit.Framework.Category("UnitTest")]
    public class PointTest : ExtendedITextTest {
        private static double EPSILON_COMPARISON = 1E-12;

        [NUnit.Framework.Test]
        public virtual void DefaultConstructorTest() {
            Point first = new Point();
            NUnit.Framework.Assert.AreEqual(0, first.GetX(), EPSILON_COMPARISON);
            NUnit.Framework.Assert.AreEqual(0, first.GetY(), EPSILON_COMPARISON);
        }

        [NUnit.Framework.Test]
        public virtual void CopyConstructorTest() {
            Point first = new Point(new Point(4.0, 5.0));
            NUnit.Framework.Assert.AreEqual(4.0, first.GetX(), EPSILON_COMPARISON);
            NUnit.Framework.Assert.AreEqual(5.0, first.GetY(), EPSILON_COMPARISON);
        }

        [NUnit.Framework.Test]
        public virtual void DoubleParamConstructorTest() {
            Point first = new Point(0.13, 1.1);
            NUnit.Framework.Assert.AreEqual(0.13, first.GetX(), EPSILON_COMPARISON);
            NUnit.Framework.Assert.AreEqual(1.1, first.GetY(), EPSILON_COMPARISON);
        }

        [NUnit.Framework.Test]
        public virtual void IntParamConstructorTest() {
            Point first = new Point(2, 3);
            NUnit.Framework.Assert.AreEqual(2, first.GetX(), EPSILON_COMPARISON);
            NUnit.Framework.Assert.AreEqual(3, first.GetY(), EPSILON_COMPARISON);
        }

        [NUnit.Framework.Test]
        public virtual void EqualsItselfTest() {
            Point first = new Point(1.23, 1.1);
            NUnit.Framework.Assert.IsTrue(first.Equals(first));
            NUnit.Framework.Assert.AreEqual(first.GetHashCode(), first.GetHashCode());
        }

        [NUnit.Framework.Test]
        public virtual void EqualsToAnotherPointTest() {
            Point first = new Point(1.23, 1.1);
            Point second = new Point(1.23, 1.1);
            NUnit.Framework.Assert.IsTrue(first.Equals(second));
            NUnit.Framework.Assert.IsTrue(second.Equals(first));
            NUnit.Framework.Assert.AreEqual(first.GetHashCode(), second.GetHashCode());
        }

        [NUnit.Framework.Test]
        public virtual void NotEqualsToAnotherPointTest() {
            Point first = new Point(1.23, 1.1);
            Point second = new Point(1.23, 1.2);
            NUnit.Framework.Assert.IsFalse(first.Equals(second));
            NUnit.Framework.Assert.IsFalse(second.Equals(first));
            NUnit.Framework.Assert.AreNotEqual(first.GetHashCode(), second.GetHashCode());
        }

        [NUnit.Framework.Test]
        public virtual void NotEqualsToNullTest() {
            Point first = new Point(1.23, 1.1);
            NUnit.Framework.Assert.IsFalse(first.Equals(null));
        }

        [NUnit.Framework.Test]
        public virtual void DistanceByCoordinatesTest() {
            Point first = new Point(1, 1);
            Point second = new Point(1.1, 1.1);
            double expected = Math.Sqrt(0.02);
            NUnit.Framework.Assert.AreEqual(expected, first.Distance(second.GetX(), second.GetY()), EPSILON_COMPARISON
                );
        }

        [NUnit.Framework.Test]
        public virtual void DistanceByPointTest() {
            Point first = new Point(1, 1);
            Point second = new Point(1.1, 1.1);
            double expected = Math.Sqrt(0.02);
            NUnit.Framework.Assert.AreEqual(expected, first.Distance(second), EPSILON_COMPARISON);
        }

        [NUnit.Framework.Test]
        public virtual void DistanceItselfTest() {
            Point first = new Point(1, 1);
            NUnit.Framework.Assert.AreEqual(0, first.Distance(first), EPSILON_COMPARISON);
        }

        [NUnit.Framework.Test]
        public virtual void ToStringTest() {
            Point first = new Point(1.23, 1.1);
            NUnit.Framework.Assert.AreEqual("Point: [x=1.23,y=1.1]", first.ToString());
        }

        [NUnit.Framework.Test]
        public virtual void CloneTest() {
            Point first = new Point(1.23, 1.1);
            Point clone = (Point)first.Clone();
            NUnit.Framework.Assert.AreEqual(first, clone);
            NUnit.Framework.Assert.AreEqual(first.GetHashCode(), clone.GetHashCode());
        }

        [NUnit.Framework.Test]
        public virtual void MoveTest() {
            float w = 3.73f;
            float h = 5.23f;
            Rectangle rectangle = new Rectangle(0, 0, w, h);
            Point[] expectedPoints = rectangle.ToPointsArray();
            Point point = new Point(0, 0);
            point.Move(w, 0);
            NUnit.Framework.Assert.AreEqual(expectedPoints[1], point);
            point.Move(0, h);
            NUnit.Framework.Assert.AreEqual(expectedPoints[2], point);
            point.Move(-w, 0);
            NUnit.Framework.Assert.AreEqual(expectedPoints[3], point);
            point.Move(0, -h);
            NUnit.Framework.Assert.AreEqual(expectedPoints[0], point);
        }

        [NUnit.Framework.Test]
        public virtual void PointVsItLocationTest() {
            Point first = new Point(1.23, 1.1);
            Point location = first.GetLocation();
            NUnit.Framework.Assert.IsTrue(first != location && first.Equals(location));
        }

        [NUnit.Framework.Test]
        public virtual void SetLocationByDoubleParamTest() {
            Point first = new Point(1.23, 1.1);
            Point second = new Point(3.59, 0.87);
            NUnit.Framework.Assert.AreNotEqual(first, second);
            first.SetLocation(second.GetX(), second.GetY());
            NUnit.Framework.Assert.AreEqual(first, second);
        }
    }
}
