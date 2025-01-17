package Clases;


import com.fasterxml.jackson.annotation.JsonIgnoreProperties;
import com.fasterxml.jackson.annotation.JsonProperty;

@JsonIgnoreProperties(ignoreUnknown = true)
public class RefCustomerStatus {

    @JsonProperty("ID")
    private String ID;
    @JsonProperty("NAME")
    private String NAME;
    @JsonProperty("SOURCE_ID")
    private String SOURCE_ID;

    public String getID() {
        return ID;
    }

    public void setID(String value) {
        this.ID = value;
    }

    public String getNAME() {
        return NAME;
    }

    public void setNAME(String value) {
        this.NAME = value;
    }

    public String getSOURCE_ID() {
        return SOURCE_ID;
    }

    public void setSOURCE_ID(String value) {
        this.SOURCE_ID = value;
    }
}
