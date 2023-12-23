namespace Aspirate.DockerCompose.Emitters;

public sealed class YamlIEnumerableSkipEmptyObjectGraphVisitor(IObjectGraphVisitor<IEmitter> nextVisitor)
    : ChainedObjectGraphVisitor(nextVisitor)
{
    public override bool EnterMapping(IPropertyDescriptor key, IObjectDescriptor value, IEmitter context)
    {
        var retVal = false;

        if (value.Value == null)
        {
            return false;
        }

        if (value.Value is IEnumerable enumerableObject)
        {
            if (enumerableObject.GetEnumerator().MoveNext()) // Returns true if the collection is not empty.
            {
                retVal = base.EnterMapping(key, value, context);
            }
        }
        else
        {
            retVal = base.EnterMapping(key, value, context);
        }

        return retVal;
    }
}
