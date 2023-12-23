namespace Aspirate.DockerCompose.Emitters;

public class FlowStringEnumConverter(IEventEmitter nextEmitter) : ChainedEventEmitter(nextEmitter)
{
    public override void Emit(ScalarEventInfo eventInfo, IEmitter emitter)
    {
        if (eventInfo.Source.Type is { IsEnum: true } sourceType && eventInfo.Source.Value is { } value)
        {
            var enumMember = sourceType.GetMember(value.ToString()!).FirstOrDefault();
            var yamlValue = enumMember?.GetCustomAttributes<EnumMemberAttribute>(true).Select(ema => ema.Value).FirstOrDefault() ?? value.ToString();

            eventInfo = new(new ObjectDescriptor(
                yamlValue,
                typeof(string),
                typeof(string),
                eventInfo.Source.ScalarStyle
            ));
        }

        nextEmitter.Emit(eventInfo, emitter);
    }
}
