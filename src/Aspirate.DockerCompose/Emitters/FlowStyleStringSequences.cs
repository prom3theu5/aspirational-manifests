namespace Aspirate.DockerCompose.Emitters;

public class FlowStyleStringSequences(IEventEmitter nextEmitter) : ChainedEventEmitter(nextEmitter)
{
    public override void Emit(SequenceStartEventInfo eventInfo, IEmitter emitter)
    {
        if (typeof(string[]) == eventInfo.Source.Type)
        {
            eventInfo = new(eventInfo.Source)
            {
                Style = SequenceStyle.Flow,
            };
        }

        nextEmitter.Emit(eventInfo, emitter);
    }
}
