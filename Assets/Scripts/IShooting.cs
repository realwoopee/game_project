
public interface IShooting
{
    bool Ads { get; }
    void fire();
    void hipfire();
    void Shoot()
    {
        if (Ads)
            fire();
        else
            hipfire();
    }
}