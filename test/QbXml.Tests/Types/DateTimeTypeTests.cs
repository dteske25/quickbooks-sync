﻿using NUnit.Framework;
using QbSync.QbXml.Objects;
using QbSync.QbXml.Tests.Helpers;
using System;
using System.IO;
using System.Xml;

namespace QbSync.QbXml.Tests.Types
{
    [TestFixture]
    public class DateTimeTypeTests
    {
        [TestCase("2015-04-03T10:06:17-08:00", ExpectedResult = "2015-04-03T10:06:17-08:00")]
        [TestCase("2015-04-03T10:06:17-07:00", ExpectedResult = "2015-04-03T10:06:17-07:00")]
        [TestCase("2015-04-03T10:06:17Z", ExpectedResult = "2015-04-03T10:06:17+00:00", Description = "UTC Z offset format will change to +00:00")]
        [TestCase("2015-04-03T10:06:17", ExpectedResult = "2015-04-03T10:06:17", Description = "UTC Offset is optional")]
        public string ToStringOffsetExistenceMatchesInputWhenParsingFromString(string input)
        {
            return DATETIMETYPE.Parse(input).ToString();
        }

        [TestCase("2015-04-03T10:06:17-08:00", ExpectedResult = "2015-04-03T10:06:17-08:00")]
        [TestCase("2015-04-03T10:06:17-07:00", ExpectedResult = "2015-04-03T10:06:17-07:00")]
        [TestCase("2015-04-03T10:06:17Z", ExpectedResult = "2015-04-03T10:06:17+00:00", Description = "UTC Z offset format will change to +00:00")]
        [TestCase("2015-04-03T10:06:17", ExpectedResult = "2015-04-03T10:06:17", Description = "UTC Offset is optional")]
        public string ToStringOffsetExistenceMatchesInputWhenConstructingFromString(string input)
        {
#pragma warning disable 618
            return new DATETIMETYPE(input).ToString();
#pragma warning restore 618
        }

        [Test]
        public void ToStringDoesNotIncludeOffsetWhenConstructedFromUnspecifiedDateTime()
        {
            var date = new DateTime(2019, 2, 6, 17, 24, 0, DateTimeKind.Unspecified);
            var dt = new DATETIMETYPE(date);

            Assert.AreEqual("2019-02-06T17:24:00", dt.ToString());
        }

        [Test]
        public void ToStringDoesIncludesOffsetWhenConstructedFromUtcDateTime()
        {
            var date = new DateTime(2019, 2, 6, 17, 24, 0, DateTimeKind.Utc);
            var dt = new DATETIMETYPE(date);

            Assert.AreEqual("2019-02-06T17:24:00+00:00", dt.ToString());
        }

        [Test]
        public void ToStringDoesIncludesOffsetWhenConstructedFromLocalDateTime()
        {
            var date = new DateTime(2019, 2, 6, 17, 24, 0, DateTimeKind.Local);

            //Note, this will be +00:00, not Z on a UTC machine, because of DateTimeKind.Local
            var offset = date.ToString(" K").Trim(); 

            var dt = new DATETIMETYPE(date);

            Assert.AreEqual($"2019-02-06T17:24:00{offset}", dt.ToString());
        }

        [Test]
        public void MidnightIsAssumedIfTimeComponentMissing()
        {
            var dt = DATETIMETYPE.Parse("2019-02-06");
            var date = dt.ToDateTime();

            Assert.AreEqual("2019-02-06T00:00:00", dt.ToString());
            Assert.AreEqual(new DateTime(2019, 2, 6, 0, 0, 0), date);
        }

        [Test]
        public void UsesOffsetAsSuppliedWhenConstructedFromDateTimeOffset()
        {
            var dt = new DATETIMETYPE(new DateTimeOffset(2019, 2, 6, 17, 24, 0, TimeSpan.FromHours(-8)));

            Assert.AreEqual("2019-02-06T17:24:00-08:00", dt.ToString());
        }

        [Test]
        public void UsesOffsetAsSuppliedWhenConstructedFromDateComponents()
        {
            var dt = new DATETIMETYPE(2019, 2, 6, 17, 24, 0, TimeSpan.FromHours(-8));

            Assert.AreEqual("2019-02-06T17:24:00-08:00", dt.ToString());
        }

        [Test]
        public void UsesNoOffsetWhenConstructedFromDateComponentsWithNullOffset()
        {
            var dt = new DATETIMETYPE(2019, 2, 6, 17, 24, 0, null);

            Assert.AreEqual("2019-02-06T17:24:00", dt.ToString());
        }

#pragma warning disable 618
        [Test]
        public void ObsoleteImplicitCastFromDateTime()
        {
            DATETIMETYPE dt = new DateTime(2019, 2, 6, 17, 24, 0, DateTimeKind.Unspecified);
            Assert.AreEqual("2019-02-06T17:24:00", dt.ToString());
        }

        [Test]
        public void ObsoleteExplicitCastFromDateTime()
        {
            var dt = (DATETIMETYPE)new DateTime(2019, 2, 6, 17, 24, 0, DateTimeKind.Unspecified);
            Assert.AreEqual("2019-02-06T17:24:00", dt.ToString());
        }
#pragma warning restore 618

        [Test]
        public void CompareAccountsForOffset()
        {
            //A's instant (moment in time globally) is later, but its DateTime is earlier
            var a = new DATETIMETYPE(new DateTimeOffset(2019, 1, 1, 6, 0, 0, 0, TimeSpan.FromHours(-3)));

            //B's instant is earlier, but its DateTime is later
            var b = new DATETIMETYPE(new DateTimeOffset(2019, 1, 1, 8, 0, 0, 0, TimeSpan.Zero));

            Assert.AreEqual(1, a.CompareTo(b));
        }

        [Test]
        public void CompareIgnoresOffsetWhenOneDoNotHaveOffset()
        {
            var a = new DATETIMETYPE(new DateTimeOffset(2019, 1, 1, 8, 0, 0, 0, TimeSpan.FromHours(-10)));
            var b = new DATETIMETYPE(new DateTime(2019, 1, 1, 8, 0, 0, 0, DateTimeKind.Unspecified));

            Assert.AreEqual(0, a.CompareTo(b));
        }

        [Test]
        public void EqualsOperatorSameInstance()
        {
            var a = new DATETIMETYPE(2019, 1, 1);
            var b = a;

            Assert.IsTrue(a == b);
        }

        [Test]
        public void EqualsOperatorSameValue()
        {
            var a = new DATETIMETYPE(2019, 1, 1);
            var b = new DATETIMETYPE(2019, 1, 1);

            Assert.IsTrue(a == b);
        }

        [Test]
        public void EqualsOperatorSameValueDifferentConstruction()
        {
            var a = new DATETIMETYPE(2019, 1, 1);
            var b = new DATETIMETYPE(new DateTime(2019, 1, 1));

            Assert.IsTrue(a == b);
        }

        [Test]
        public void EqualsOperatorBothNull()
        {
            DATETIMETYPE a = null;
            DATETIMETYPE b = null;

            Assert.IsTrue(a == b);
        }

        [Test]
        public void EqualsOperatorLeftNull()
        {
            DATETIMETYPE a = null;
            DATETIMETYPE b = new DATETIMETYPE(2019, 1, 1);

            Assert.IsFalse(a == b);
        }

        [Test]
        public void EqualsOperatorRightNull()
        {
            DATETIMETYPE a = new DATETIMETYPE(2019, 1, 1);
            DATETIMETYPE b = null;

            Assert.IsFalse(a == b);
        }

        [Test]
        public void EqualsSameInstantDifferentOffsets()
        {
            //Even though these are two different offsets, they represent the same moment in time and both have offsets supplied, so should be equal

            var a = new DATETIMETYPE(2019, 1, 1, 12, 0, 0, TimeSpan.FromHours(-1));
            var b = new DATETIMETYPE(2019, 1, 1, 11, 0, 0, TimeSpan.FromHours(-2));

            Assert.AreEqual(0, a.CompareTo(b));
            Assert.IsTrue(a.Equals(b));
        }

        [Test]
        public void EqualsOperatorSameInstantDifferentOffsets()
        {
            //Even though these are two different offsets, they represent the same moment in time and both have offsets supplied, so should be equal

            var a = new DATETIMETYPE(2019, 1, 1, 12, 0, 0, TimeSpan.FromHours(-1));
            var b = new DATETIMETYPE(2019, 1, 1, 11, 0, 0, TimeSpan.FromHours(-2));

            Assert.AreEqual(0, a.CompareTo(b));
            Assert.IsTrue(a == b);
        }

        [Test]
        public void EqualsOperatorSameTimeOneMissingOffset()
        {
            //While these will compare the same, they should not be considered equal

            var a = new DATETIMETYPE(new DateTimeOffset(2019, 1, 1, 8, 0, 0, 0, TimeSpan.Zero));
            var b = new DATETIMETYPE(new DateTime(2019, 1, 1, 8, 0, 0, 0, DateTimeKind.Unspecified));

            Assert.AreEqual(0, a.CompareTo(b));
            Assert.IsFalse(a == b);
        }

        private static DATETIMETYPE[] ToDateTimeIsUtcForZeroOffsetsInput()
        {
            return new[]
            {
                DATETIMETYPE.Parse("2019-02-06T17:24:00Z"),
                DATETIMETYPE.Parse("2019-02-06T17:24:00+00:00"),
                new DATETIMETYPE(new DateTime(2019, 1, 1, 8, 0, 0, 0, DateTimeKind.Utc)),
                new DATETIMETYPE(new DateTimeOffset(2019, 2, 6, 17, 24, 0, TimeSpan.Zero)),
            };
        }

        [Test, TestCaseSource(nameof(ToDateTimeIsUtcForZeroOffsetsInput))]
        public void ToDateTimeIsUtcForZeroOffsets(DATETIMETYPE input)
        {
            Assert.AreEqual(DateTimeKind.Utc, input.ToDateTime().Kind);
        }


        private static DATETIMETYPE[] ToDateTimeIsUnspecifiedForNonZeroAndEmptyOffsetsInputs()
        {
            return new[]
            {
                DATETIMETYPE.Parse("2019-02-06T17:24:00"),
                DATETIMETYPE.Parse("2019-02-07T17:24:00-08:00"),
                new DATETIMETYPE(new DateTime(2019, 1, 1, 8, 0, 0, 0, DateTimeKind.Unspecified)),
                new DATETIMETYPE(new DateTimeOffset(2019, 2, 9, 17, 24, 0, TimeSpan.FromHours(-8)))
            };
        }

        [Test, TestCaseSource(nameof(ToDateTimeIsUnspecifiedForNonZeroAndEmptyOffsetsInputs))]
        public void ToDateTimeIsUnspecifiedForNonZeroAndEmptyOffsets(DATETIMETYPE input)
        {
            Assert.AreEqual(DateTimeKind.Unspecified, input.ToDateTime().Kind);
        }

        [Test]
        public void ToDateTimeIsLocalKindWhenConstructedFromLocalDateTime()
        {
            //This situation only covers when a consumer of this library is converting back to DateTime after initializing from DateTime
            //No QuickBooks parsed DATETIMETYPE will ever have a Local kind

            var dt = new DATETIMETYPE(new DateTime(2019, 1, 1, 8, 0, 0, 0, DateTimeKind.Local));
            Assert.AreEqual(DateTimeKind.Local, dt.ToDateTime().Kind);
        }

        [Test]
        public void ToDateTimeEqualsInputWhenConstructedFromLocalDateTime()
        {
            var input = new DateTime(2018, 8, 1, 0, 0, 0, DateTimeKind.Local);
            var dt = new DATETIMETYPE(input);
            var output = dt.ToDateTime();

            Assert.AreEqual(input, output);
        }

        [Test]
        public void OffsetMatchesLocalMachineZoneWhenConstructedFromLocalDateTime()
        {
            var localDate = new DateTime(2019, 1, 1, 8, 0, 0, 0, DateTimeKind.Local);
            var zone = TimeZoneInfo.Local;
            var offset = zone.GetUtcOffset(localDate);

            var dt = new DATETIMETYPE(new DateTime(2019, 1, 1, 8, 0, 0, 0, DateTimeKind.Local));

            Assert.AreEqual(offset, dt.GetDateTimeOffset().Offset);
        }


        [Test]
        public void GetDateTimeOffsetThrowsWhenNoOffset()
        {
            var dt = DATETIMETYPE.Parse("2019-02-06T17:24:00");
            Assert.Throws<InvalidOperationException>(() => dt.GetDateTimeOffset());
        }

        [Test]
        public void GetDateTimeOffsetReturnsSameValue()
        {
            var dto = new DateTimeOffset(2019, 2, 6, 17, 24, 0, TimeSpan.FromHours(-8));
            var dt = new DATETIMETYPE(dto);

            Assert.AreEqual(dto, dt.GetDateTimeOffset());
        }

        [Test]
        public void LessThanOperator()
        {
            var a = new DATETIMETYPE(new DateTimeOffset(2019, 1, 1, 8, 0, 0, TimeSpan.Zero));
            var b = new DATETIMETYPE(new DateTimeOffset(2019, 1, 1, 9, 0, 0, TimeSpan.Zero));

            Assert.IsTrue(a < b);
        }

        [Test]
        public void GreaterThanOperator()
        {
            var a = new DATETIMETYPE(new DateTimeOffset(2019, 1, 1, 8, 0, 0, TimeSpan.Zero));
            var b = new DATETIMETYPE(new DateTimeOffset(2019, 1, 1, 9, 0, 0, TimeSpan.Zero));

            Assert.IsTrue(b > a);
        }


        [Test]
        public void ThrowsWhenConstructedWithDateTimeAfterEpoch()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                var date = new DATETIMETYPE(new DateTime(2038, 2, 1, 0, 0, 0));
            });
        }

        [Test]
        public void ThrowsWhenConstructedWithDateTimeBefore1970()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                var date = new DATETIMETYPE(new DateTime(1969, 12, 31, 23, 59, 59));
            });
        }

        [Test]
        public void ThrowsWhenConstructedWithDateTimeOffsetAfterEpoch()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                var date = new DATETIMETYPE(new DateTimeOffset(2038, 2, 1, 0, 0, 0, TimeSpan.Zero));
            });
        }

        [Test]
        public void ThrowsWhenConstructedWithDateTimeOffsetBefore1970()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                var date = new DATETIMETYPE(new DateTimeOffset(1969, 12, 31, 23, 59, 59, TimeSpan.Zero));
            });
        }

        [Test]
        public void ThrowsWhenConstructedWithYearOn2038()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                var date = new DATETIMETYPE(2038, 1, 1);
            });
        }

        [Test]
        public void ThrowsWhenConstructedWithYearBefore1970()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                var date = new DATETIMETYPE(1969, 12, 31, 23, 59, 59);
            });
        }

        [Test]
        public void DoesNotThrowWhenConstructedWithLastDayOf2037()
        {
            Assert.DoesNotThrow(() =>
            {
                var date = new DATETIMETYPE(2037, 12, 31, 23, 59, 59);
            });
        }

        [Test]
        public void DoesNotThrowWhenConstructedWithFirstDayOf1970()
        {
            Assert.DoesNotThrow(() =>
            {
                var date = new DATETIMETYPE(1970, 01, 01, 0, 0, 0);
            });
        }

        [Test]
        public void DoesNotThrowWhenConstructedWithFirstDayOf1970AndOffset()
        {
            Assert.DoesNotThrow(() =>
            {
                var date = new DATETIMETYPE(1970, 01, 01, 0, 0, 0, TimeSpan.Zero);
            });
        }

        [Test]
        public void DoesNotThrowWhenConstructedWithDateTimeOffsetOnEpoch()
        {
            Assert.DoesNotThrow(() =>
            {
                var date = new DATETIMETYPE(new DateTimeOffset(2038, 1, 19, 3, 14, 7, 0, TimeSpan.Zero));
            });
        }

        [Test]
        public void ThrowsWhenParsedFromStringAfterQbEpoch()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                var dt = DATETIMETYPE.Parse("2038-01-19T03:14:08+00:00");
            });
        }

        [Test]
        public void DoesNotThrowWhenParsedFromStringOnQbEpoch()
        {
            Assert.DoesNotThrow(() =>
            {
                var dt = DATETIMETYPE.Parse("2038-01-19T03:14:07+00:00");
            });
        }

        [TestCase("0000-00-00")]
        [TestCase("0000-00-00T00:00:00")]
        [TestCase("")]
        public void ThrowsFormatExceptionWhenInvalidStringIsParsed(string dateString)
        {
            Assert.Throws<FormatException>(() =>
            {
                var dt = DATETIMETYPE.Parse(dateString);
            });
        }

        [Test]
        public void ThrowsExceptionWhenNullStringIsParsed()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                var dt = DATETIMETYPE.Parse(null);
            });
        }


        private static DATETIMETYPE[] ReadXmlThrowsInputs()
        {
            //Return one DATETIMETYPE for each construction method

            return new[]
            {
                new DATETIMETYPE(2019, 1, 1),
                new DATETIMETYPE(new DateTime(2019, 1, 2)),
                new DATETIMETYPE(new DateTimeOffset(2019, 1, 3, 0, 0, 0, TimeSpan.Zero)),
                DATETIMETYPE.Parse("2019-01-04T00:00:00")
            };
        }

        [Test, TestCaseSource(nameof(ReadXmlThrowsInputs))]
        public void ReadXmlThrows(DATETIMETYPE date)
        {
            //IXmlSerializable requires ReadXML, which makes the class difficult to make immutable
            //To ensure as much immutability as possible, only let the deserializer use ReadXML

            var reader = XmlReader.Create(new MemoryStream());

            Assert.Throws<InvalidOperationException>(() =>
            {
                date.ReadXml(reader);
            });
        }


        #region QuickBooks parsing and fixes

        private CustomerRet CreateAndParseCustomerQueryXml(string timeCreated, string timeModified, TimeZoneInfo quickBooksTimeZone = null)
        {
            var ret = $"<CustomerRet><ListID>80000001-1422671082</ListID><TimeCreated>{timeCreated}</TimeCreated><TimeModified>{timeModified}</TimeModified><EditSequence>1422671082</EditSequence><Name>Chris Curtis</Name><FullName>Christopher Curtis</FullName><IsActive>true</IsActive></CustomerRet>";

            var response = new QbXmlResponse(new QbXmlResponseOptions
            {
                TimeZoneBugFix = quickBooksTimeZone
            });
            var rs = response.GetSingleItemFromResponse<CustomerQueryRsType>(QuickBooksTestHelper.CreateQbXmlWithEnvelope(ret, "CustomerQueryRs"));
            return rs.CustomerRet[0];
        }


        [Test]
        public void OffsetIsIgnoreWhenXmlParsed()
        {
            //This tests the QuickBooks fix that simply ignores the returned offset portion of the date [when no fix applied]

            var time = "2015-04-03T10:06:17-07:00";

            var customer = CreateAndParseCustomerQueryXml(time, time);

            Assert.AreEqual("2015-04-03T10:06:17", customer.TimeModified.ToString());
        }

        [Test]
        public void IncorrectOffsetReturnedFromQuickBooksDoesNotAlterParsedTime()
        {
            var incorrectOffset = "2015-04-03T10:06:17-08:00";
            var correctOffset = "2015-04-03T10:06:17-07:00";

            var customer = CreateAndParseCustomerQueryXml(timeCreated: incorrectOffset, timeModified: correctOffset);

            Assert.AreEqual("2015-04-03T10:06:17", customer.TimeCreated.ToString());
            Assert.AreEqual("2015-04-03T10:06:17", customer.TimeModified.ToString());
        }

        [Test]
        public void DateTimeOnXmlParsedDateIsUnspecifiedKind()
        {
            var time = "2015-04-03T10:06:17-08:00";

            var customer = CreateAndParseCustomerQueryXml(time, time);

            Assert.AreEqual(DateTimeKind.Unspecified, customer.TimeModified.ToDateTime().Kind);
        }

        [Test]
        public void OffsetIsSetToTimeZoneCorrectValueWhenXmlParsedWithTimeZoneFix()
        {
            var time = "2015-04-03T10:06:17-08:00";

            var customer = CreateAndParseCustomerQueryXml(time, time, QuickBooksTestHelper.GetPacificStandardTimeZoneInfo());

            Assert.AreEqual("2015-04-03T10:06:17-07:00", customer.TimeModified.ToString());
        }

        [Test]
        public void OffsetIsRetainedWhenXmlParsedForNonDstTimeZoneFix()
        {

            var time = "2015-04-03T10:06:17+00:00";

            var customer = CreateAndParseCustomerQueryXml(time, time, TimeZoneInfo.Utc);

            Assert.AreEqual("2015-04-03T10:06:17+00:00", customer.TimeModified.ToString());
        }

        [Test]
        public void OffsetIsIgnoredWhenXmlParsedAndTimeZoneFixBaseOffsetDoesNotMatchInput()
        {
            //A protection against misconfiguration if the time zone is set completely wrong

            var time = "2015-04-03T10:06:17-10:00";

            var customer = CreateAndParseCustomerQueryXml(time, time, QuickBooksTestHelper.GetPacificStandardTimeZoneInfo());

            Assert.AreEqual("2015-04-03T10:06:17", customer.TimeModified.ToString());
        }

        [Test]
        public void DefaultDateTimeUsedWhenXmlParsedWithInvalidDate()
        {
            //Dates have been seen with 0000-00-00. Need to use default date instead of throwing

            var time = "0000-00-00";

            var customer = CreateAndParseCustomerQueryXml(time, time);

            Assert.AreEqual("1970-01-01T00:00:00", customer.TimeModified.ToString());
        }

        [Test]
        public void DefaultDateTimeUsedWhenXmlParsedWithEmptyDate()
        {
            var time = "";

            var customer = CreateAndParseCustomerQueryXml(time, time);

            Assert.AreEqual("1970-01-01T00:00:00", customer.TimeModified.ToString());
        }


        #endregion
    }
}
