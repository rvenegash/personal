package Clases;

import com.fasterxml.jackson.annotation.JsonIgnoreProperties;
import com.fasterxml.jackson.annotation.JsonProperty;

@JsonIgnoreProperties(ignoreUnknown = true)
public class Stg_Customer { //implements JSONSerdeCompatible

    @JsonProperty("table")
    private String table;
    @JsonProperty("op_type")
    private String op_type;
    @JsonProperty("op_ts")
    private String op_ts;
    @JsonProperty("current_ts")
    private String current_ts;
    @JsonProperty("pos")
    private String pos;
    @JsonProperty("primary_keys")
    private String[] primary_keys;
    @JsonProperty("after")
    private Stg_Customer_After after;
    public String gettable() {
        return table;
    }
    public void settable(String value) {
        this.table = value;
    }


    public String getOp_type() {
        return op_type;
    }
    public void setOp_type(String value) {
        this.op_type = value;
    }


    public String getOp_ts() {
        return op_ts;
    }
    public void setOp_ts(String value) {
        this.op_ts = value;
    }


    public String getCurrent_ts() {
        return current_ts;
    }
    public void setCurrent_ts(String value) {
        this.current_ts = value;
    }


    public String getPos() {
        return pos;
    }
    public void setPos(String value) {
        this.pos = value;
    }


    public String[] getPrimary_keys() {
        return primary_keys;
    }
    public void setPrimary_keys(String[] value) {
        this.primary_keys = value;
    }


    public Stg_Customer_After getAfter() {
        return after;
    }
    public void setAfter(Stg_Customer_After value) {
        this.after = value;
    }
}
