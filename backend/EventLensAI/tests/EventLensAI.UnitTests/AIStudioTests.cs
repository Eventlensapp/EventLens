using EventLensAI.Application.Features.AI;
using EventLensAI.Domain.Entities;
using EventLensAI.Domain.Enums;

namespace EventLensAI.UnitTests;

public sealed class AIStudioTests
{
    [Fact]
    public void Job_tracks_progress_and_output()
    {
        var job=new AIJob(Guid.NewGuid(),null,Guid.NewGuid(),AIJobType.BackgroundRemoval,
            "configured-provider",null,null,"storage/input.jpg",Guid.NewGuid());
        job.Start();job.ReportProgress(55);job.Complete("storage/output.png");
        Assert.Equal(AIJobStatus.Completed,job.Status);Assert.Equal(100,job.Progress);
        Assert.Equal("storage/output.png",job.OutputImage);
    }
    [Fact]
    public void Failed_job_can_be_requeued()
    {
        var job=new AIJob(Guid.NewGuid(),null,null,AIJobType.StyleTransfer,"provider","anime",null,"input.jpg",Guid.NewGuid());
        job.Start();job.Fail("temporary",true);job.Requeue();
        Assert.Equal(AIJobStatus.Queued,job.Status);Assert.Equal(1,job.RetryCount);
    }
    [Theory]
    [InlineData(2,true)][InlineData(4,true)][InlineData(8,true)][InlineData(3,false)]
    public async Task Validator_enforces_supported_upscale_factors(int factor,bool expected)
    {
        var request=new CreateAIJobRequest(Guid.NewGuid(),null,null,"input.jpg",null,null,null,null,null,factor,null);
        Assert.Equal(expected,(await new CreateAIJobRequestValidator().ValidateAsync(request)).IsValid);
    }
}
