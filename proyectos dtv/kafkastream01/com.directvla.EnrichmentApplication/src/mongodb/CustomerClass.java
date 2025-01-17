package mongodb;

public class CustomerClass
{
    private String _id;
    private String id;
    private String NAME;
    private String SOURCE_ID;

    public String get_id() {
        return this._id;
    }
    public void set_id(final String value) {
        this._id = value;
    }

    public String getid() {
        return this.id;
    }
    public void setid(final String value) {
        this.id = value;
    }

    public String getNAME() {
        return this.NAME;
    }
    public void setNAME(final String value) {
        this.NAME = value;
    }

    public String getSOURCE_ID() {
        return this.SOURCE_ID;
    }
    public void setSOURCE_ID(final String value) {
        this.SOURCE_ID = value;
    }
}
