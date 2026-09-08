using EventLensAI.Domain.Common;
using EventLensAI.Domain.Enums;

namespace EventLensAI.Domain.Entities;
public sealed class AIPromptDefinition : BaseEntity
{
    private AIPromptDefinition(){}
    public AIPromptDefinition(string key,AIJobType jobType,string prompt,string? negativePrompt)
    {Key=key;JobType=jobType;Prompt=prompt;NegativePrompt=negativePrompt;}
    public string Key{get;private set;}=string.Empty;
    public AIJobType JobType{get;private set;}
    public string Prompt{get;private set;}=string.Empty;
    public string? NegativePrompt{get;private set;}
    public bool IsActive{get;private set;}=true;
}
