namespace Aspirate.DockerCompose.Emitters;

public class QuoteSurroundingEventEmitter(IEventEmitter nextEmitter) : ChainedEventEmitter(nextEmitter)
{
    public override void Emit(ScalarEventInfo eventInfo, IEmitter emitter)
    {
        if (eventInfo.Source.StaticType == typeof(string) || eventInfo.Source.StaticType == typeof(object))
        {
            eventInfo.Style = ScalarStyle.DoubleQuoted;
        }

        base.Emit(eventInfo, emitter);
    }

    public override void Emit(MappingStartEventInfo eventInfo, IEmitter emitter) => nextEmitter.Emit(eventInfo, emitter);

    public override void Emit(MappingEndEventInfo eventInfo, IEmitter emitter) => nextEmitter.Emit(eventInfo, emitter);
}
