﻿namespace GoogleMapsApi.Test.IntegrationTests
{
    using System;
    using System.Linq;
    using GoogleMapsApi.Engine;
    using GoogleMapsApi.Entities.DistanceMatrix.Request;
    using GoogleMapsApi.Entities.DistanceMatrix.Response;

    using NUnit.Framework;
    using GoogleMapsApi.Test.Utils;
    using System.Threading.Tasks;

    [TestFixture]
    public class DistanceMatrixTests : BaseTestIntegration
    {

        [Test]
        public async Task ShouldReturnValidValueWhenOneOriginAndOneDestinationsSpeciefed()
        {
            var request = new DistanceMatrixRequest
            {
                ApiKey = ApiKey,
                Origins = new[] { "49.64265,12.50088" },
                Destinations = new[] { "53.64308,10.52726" }
            };

            var result = await GoogleMaps.DistanceMatrix.QueryAsync(request);

            AssertInconclusive.NotExceedQuota(result);
            Assert.That(result.Status, Is.EqualTo(DistanceMatrixStatusCodes.OK), result.ErrorMessage);
            Assert.That(result.DestinationAddresses, Is.EqualTo(new[] { "Alter Sirksfelder Weg 10, 23881 Koberg, Germany" }));
            Assert.That(result.OriginAddresses, Is.EqualTo(new[] { "St2154 18, 92726 Waidhaus, Germany" }));
            Assert.That(result.Rows.First().Elements.First().Status, Is.EqualTo(DistanceMatrixElementStatusCodes.OK));
            Assert.That(result.Rows.First().Elements.First().Distance, Is.Not.Null);
            Assert.That(result.Rows.First().Elements.First().Duration, Is.Not.Null);
        }

        [Test]
        public async Task ShouldReturnValidValueWhenTwoOriginsSpecified()
        {
            var request = new DistanceMatrixRequest
            {
                ApiKey = ApiKey,
                Origins = new[] { "49.64265,12.50088", "49.17395,12.87028" },
                Destinations = new[] { "53.64308,10.52726" }
            };

            var result = await GoogleMaps.DistanceMatrix.QueryAsync(request);

            AssertInconclusive.NotExceedQuota(result);
            Assert.That(result.Status, Is.EqualTo(DistanceMatrixStatusCodes.OK));
            Assert.That(result.DestinationAddresses, Is.EqualTo(new[] { "Alter Sirksfelder Weg 10, 23881 Koberg, Germany" }));
            Assert.That(result.OriginAddresses, Is.EqualTo(new[] { "St2154 18, 92726 Waidhaus, Germany", "Böhmerwaldstraße 19, 93444 Bad Kötzting, Germany" }));
            Assert.That(result.Rows.Count(), Is.EqualTo(2));
            Assert.That(result.Rows.First().Elements.First().Status, Is.EqualTo(DistanceMatrixElementStatusCodes.OK));
            Assert.That(result.Rows.Last().Elements.First().Status, Is.EqualTo(DistanceMatrixElementStatusCodes.OK));
        }

        [Test]
        public async Task ShouldReturnDurationInTrafficWhenDepartureTimeAndApiKeySpecified()
        {
            var request = new DistanceMatrixRequest
            {
                ApiKey = ApiKey,
                DepartureTime = new Time(),
                Origins = new[] { "49.64265,12.50088" },
                Destinations = new[] { "53.64308,10.52726" },
            };

            var result = await GoogleMaps.DistanceMatrix.QueryAsync(request);

            AssertInconclusive.NotExceedQuota(result);
            Assert.That(result.Status, Is.EqualTo(DistanceMatrixStatusCodes.OK));
            Assert.That(result.Rows.First().Elements.First().DurationInTraffic, Is.Not.Null);
        }

        [Test]
        public void ShouldThrowExceptionWhenDepartureTimeAndArrivalTimeSpecified()
        {
            var request = new DistanceMatrixRequest
            {
                ApiKey = ApiKey,
                DepartureTime = new Time(),
                ArrivalTime = new Time(),
                Mode = DistanceMatrixTravelModes.transit,
                Origins = new[] { "49.64265,12.50088" },
                Destinations = new[] { "53.64308,10.52726" },
            };

            Assert.ThrowsAsync<ArgumentException>(() => GoogleMaps.DistanceMatrix.QueryAsync(request));
        }

        [Test]
        public void ShouldThrowExceptionWhenArrivalTimeSpecifiedForNonTransitModes()
        {
            var request = new DistanceMatrixRequest
            {
                ApiKey = ApiKey,
                ArrivalTime = new Time(),
                Mode = DistanceMatrixTravelModes.driving,
                Origins = new[] { "49.64265,12.50088" },
                Destinations = new[] { "53.64308,10.52726" },
            };

            Assert.ThrowsAsync<ArgumentException>(() => GoogleMaps.DistanceMatrix.QueryAsync(request));
        }

        [Test]
        public void ShouldThrowExceptionWheTransitRoutingPreferenceSpecifiedForNonTransitModes()
        {
            var request = new DistanceMatrixRequest
            {
                ApiKey = ApiKey,
                Mode = DistanceMatrixTravelModes.driving,
                TransitRoutingPreference = DistanceMatrixTransitRoutingPreferences.less_walking,
                Origins = new[] { "49.64265,12.50088" },
                Destinations = new[] { "53.64308,10.52726" },
            };

            Assert.ThrowsAsync<ArgumentException>(() => GoogleMaps.DistanceMatrix.QueryAsync(request));
        }

        [Test]
        public void ShouldThrowExceptionWhenTrafficModelSuppliedForNonDrivingMode()
        {
            var request = new DistanceMatrixRequest
            {
                ApiKey = ApiKey,
                Mode = DistanceMatrixTravelModes.transit,
                DepartureTime = new Time(),
                TrafficModel = DistanceMatrixTrafficModels.optimistic,
                Origins = new[] { "49.64265,12.50088" },
                Destinations = new[] { "53.64308,10.52726" },
            };

            Assert.ThrowsAsync<ArgumentException>(() => GoogleMaps.DistanceMatrix.QueryAsync(request));
        }

        [Test]
        public void ShouldThrowExceptionWhenTrafficModelSuppliedWithoutDepartureTime()
        {
            var request = new DistanceMatrixRequest
            {
                ApiKey = ApiKey,
                Mode = DistanceMatrixTravelModes.driving,
                TrafficModel = DistanceMatrixTrafficModels.optimistic,
                Origins = new[] { "49.64265,12.50088" },
                Destinations = new[] { "53.64308,10.52726" },
            };

            Assert.ThrowsAsync<ArgumentException>(() => GoogleMaps.DistanceMatrix.QueryAsync(request));
        }

        [Test]
        public void ShouldThrowExceptionWhenTransitModesSuppliedForNonTransitMode()
        {
            var request = new DistanceMatrixRequest
            {
                ApiKey = ApiKey,
                Mode = DistanceMatrixTravelModes.driving,
                TransitModes = new DistanceMatrixTransitModes[] { DistanceMatrixTransitModes.bus, DistanceMatrixTransitModes.subway},
                Origins = new[] { "49.64265,12.50088" },
                Destinations = new[] { "53.64308,10.52726" },
            };

            Assert.ThrowsAsync<ArgumentException>(() => GoogleMaps.DistanceMatrix.QueryAsync(request));
        }

        [Test]
        public async Task ShouldReturnImperialUnitsIfImperialPassedAsParameter()
        {
            var request = new DistanceMatrixRequest
            {
                ApiKey = ApiKey,
                Units = DistanceMatrixUnitSystems.imperial,
                Origins = new[] { "49.64265,12.50088" },
                Destinations = new[] { "53.64308,10.52726" },
            };

            var result = await GoogleMaps.DistanceMatrix.QueryAsync(request);

            AssertInconclusive.NotExceedQuota(result);
            Assert.That(result.Rows.First().Elements.First().Distance.Text.Contains("mi"), Is.True);
        }

        [Test]
        public async Task ShouldReplaceUriViaOnUriCreated()
        {
            var request = new DistanceMatrixRequest
            {
                ApiKey = ApiKey,
                Origins = new[] { "placeholder" },
                Destinations = new[] { "3,4" },
            };

            static Uri onUriCreated(Uri uri)
            {
                return new Uri(uri.ToString().Replace("placeholder", "1,2"));
            }

            GoogleMaps.DistanceMatrix.OnUriCreated += onUriCreated;

            try
            {
                var result = await GoogleMaps.DistanceMatrix.QueryAsync(request);

                AssertInconclusive.NotExceedQuota(result);
                Assert.That(result.Status, Is.EqualTo(DistanceMatrixStatusCodes.OK), result.ErrorMessage);
                Assert.That(result.OriginAddresses.First(), Is.EqualTo("1,2"));
            }
            finally
            {
                GoogleMaps.DistanceMatrix.OnUriCreated -= onUriCreated;
            }
        }

        [Test]
        public async Task ShouldPassRawDataToOnRawResponseReceived()
        {
            var request = new DistanceMatrixRequest
            {
                ApiKey = ApiKey,
                Origins = new[] { "placeholder" },
                Destinations = new[] { "3,4" },
            };

            var rawData = Array.Empty<byte>();

            void onRawResponseReceived(byte[] data) => rawData = data;
            GoogleMaps.DistanceMatrix.OnRawResponseReceived += onRawResponseReceived;

            try
            {
                var result = await GoogleMaps.DistanceMatrix.QueryAsync(request);

                AssertInconclusive.NotExceedQuota(result);
                Assert.That(result.Status, Is.EqualTo(DistanceMatrixStatusCodes.OK), result.ErrorMessage);
                Assert.That(rawData, Is.Not.Empty);
            }
            finally
            {
                GoogleMaps.DistanceMatrix.OnRawResponseReceived -= onRawResponseReceived;
            }
        }
    }
}
