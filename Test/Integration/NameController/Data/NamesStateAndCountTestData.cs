using System.Collections;
using System.Collections.Generic;
using Core.Entities;
using Core.Entities.NameEntry;
using Core.Entities.NameEntry.Collections;
using Core.Enums;

namespace Test.Integration.NameController.Data;

public class NamesStateAndCountTestData : IEnumerable<object[]>
{
    public IEnumerator<object[]> GetEnumerator()
    {
        yield return new object[] {NameEntries(), State.NEW, 1};
        yield return new object[] {NameEntries(), State.MODIFIED, 5 };
        yield return new object[] {NameEntries(), State.PUBLISHED, 6 };
        yield return new object[] {NameEntries(), State.UNPUBLISHED, 8 };
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    private List<NameEntry> NameEntries()
    {
        return new List<NameEntry>
        {
            new NameEntry
            {
                Name = "Ibironke",
                Meaning = "The man of valor",
                Morphology = new List<string> { "He", "Ho" },
                Media = new List<string> { "Me", "Dia" },
                State = State.PUBLISHED,
                Etymology = new List<Etymology>
                {
                    new Etymology(part: "Part1", meaning: "Meaning 1")
                },
                Videos = new List<EmbeddedVideo>
                {
                    new EmbeddedVideo(videoId: "Video ID 1", caption: "Caption 1")
                },
                GeoLocation = new List<GeoLocation>
                {
                    new GeoLocation(place: "Lagos", region: "South-West")
                }
            },
            new NameEntry
            {
                Name = "Aderonke",
                Meaning = "The man of valor",
                Morphology = new List<string> { "He", "Ho" },
                Media = new List<string> { "Me", "Dia" },
                State = State.PUBLISHED,
                Etymology = new List<Etymology>
                {
                    new Etymology(part: "Part1", meaning: "Meaning 1")
                },
                Videos = new List<EmbeddedVideo>
                {
                    new EmbeddedVideo(videoId: "Video ID 1", caption: "Caption 1")
                },
                GeoLocation = new List<GeoLocation>
                {
                    new GeoLocation(place: "Lagos", region: "South-West")
                }
            }
        };
    }
}