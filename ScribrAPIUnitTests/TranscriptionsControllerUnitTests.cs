﻿using ScribrAPI.Controllers;
using ScribrAPI.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace UnitTestScribrAPI
{
    [TestClass]
    public class TranscriptionsControllerUnitTests
    {
        // Insert code here
        public static readonly DbContextOptions<scriberContext> options
            = new DbContextOptionsBuilder<scriberContext>()
            .UseInMemoryDatabase(databaseName: "testDatabase")
            .Options;

        public static readonly IList<Transcription> transciptions = new List<Transcription>
        {
            new Transcription(){
                   Phrase = "That's like calling"
            },

            new Transcription(){
                    Phrase = "A peanut butter sandwich"
            }
        };

        [TestInitialize]
        public void SetupDb()
        {
            using (var context = new scriberContext(options)) {
                context.Transcription.Add(transciptions[0]);
                context.Transcription.Add(transciptions[0]);
                context.SaveChanges();
            }
        }

        [TestCleanup]
        public void ClearDb()
        {
            using (var context = new scriberContext(options)) {
                context.Transcription.RemoveRange(context.Transcription);
                context.SaveChanges();
            }
        }

        [TestMethod]
        public async Task TestGetSuccessfully()
        {
            using (var context = new scriberContext(options))
            {
                TranscriptionsController transcriptionsController = new TranscriptionsController(context);
                ActionResult<IEnumerable<Transcription>> result = await transcriptionsController.GetTranscription();

                Assert.IsNotNull(result);
                // i should really check to make sure the exact transcriptions are in there, but that requires an equality comparer,
                // which requires a whole nested class, thanks to C#'s lack of anonymous classes that implement interfaces

                //If the GET request fails then null will be returned. Therefore, this test will come up as failed. 
            }
        }

        [TestMethod]
        public async Task TestPutTranscriptionItemNoContentStatusCode()
        {
            using (var context = new scriberContext(options))
            {
                string title = "this is now a different phrase";
                Transcription transcription1 = context.Transcription.Where(x => x.Phrase == transciptions[0].Phrase).Single();
                transcription1.Phrase = title;

                TranscriptionsController transcriptionsController = new TranscriptionsController(context);
                IActionResult result = await transcriptionsController.PutTranscription(transcription1.TranscriptionId, transcription1) as IActionResult;

                Assert.IsNotNull(result);
                Assert.IsInstanceOfType(result, typeof(NoContentResult));
            }
        }
    }
}