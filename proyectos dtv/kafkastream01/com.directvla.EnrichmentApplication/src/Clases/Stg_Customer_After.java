package Clases;

import com.fasterxml.jackson.annotation.JsonIgnoreProperties;
import com.fasterxml.jackson.annotation.JsonProperty;

@JsonIgnoreProperties(ignoreUnknown = true)
public class Stg_Customer_After {

    @JsonProperty("ID")
    private String ID;
    @JsonProperty("SOURCE_ID")
    private String SOURCE_ID;
    @JsonProperty("ENTITY_ID")
    private String ENTITY_ID;
    @JsonProperty("CHANGE_OPERATION_ID")
    private String CHANGE_OPERATION_ID;
    @JsonProperty("CHANGE_OPERATION_TIMESTAMP")
    private String CHANGE_OPERATION_TIMESTAMP;
    @JsonProperty("TYPE_ID")
    private String TYPE_ID;
    @JsonProperty("STATUS_ID")
    private String STATUS_ID;
    @JsonProperty("CLASS_ID")
    private String CLASS_ID;
    @JsonProperty("SEGMENTATION_ID")
    private String SEGMENTATION_ID;
    @JsonProperty("CUSTOMER_SINCE")
    private String CUSTOMER_SINCE;
    @JsonProperty("EXEMPTION_CODE_ID")
    private String EXEMPTION_CODE_ID;
    @JsonProperty("EXEMPTION_SERIAL_NUMBER")
    private String EXEMPTION_SERIAL_NUMBER;
    @JsonProperty("EXEMPTION_FROM")
    private String EXEMPTION_FROM;
    @JsonProperty("REFERENCE_TYPE_ID")
    private String REFERENCE_TYPE_ID;
    @JsonProperty("IDENTIFICATION_ID")
    private String IDENTIFICATION_ID;
    @JsonProperty("INTERNET_PASSWORD_ENC")
    private String INTERNET_PASSWORD_ENC;
    @JsonProperty("INTERNET_PASSWORD_IV")
    private String INTERNET_PASSWORD_IV;
    @JsonProperty("INTERNET_USER_NAME")
    private String INTERNET_USER_NAME;
    @JsonProperty("LANGUAGE_ID")
    private String LANGUAGE_ID;
    @JsonProperty("BIRTH_DATE")
    private String BIRTH_DATE;
    @JsonProperty("BUSINESS_UNIT_ID")
    private String BUSINESS_UNIT_ID;
    @JsonProperty("MAGAZINES")
    private String MAGAZINES;
    @JsonProperty("COUNTRY_ID")
    private String COUNTRY_ID;

    public String getENTITY_ID() {
        return ENTITY_ID;
    }
    public void setENTITY_ID(String value) {
        this.ENTITY_ID = value;
    }


    public String getId() {
        return ID;
    }
    public void setId(String value) {
        this.ID = value;
    }


    public String getSOURCE_ID() {
        return SOURCE_ID;
    }
    public void setSOURCE_ID(String value) {
        this.SOURCE_ID = value;
    }


    public String getCHANGE_OPERATION_ID() {
        return CHANGE_OPERATION_ID;
    }
    public void setCHANGE_OPERATION_ID(String value) {
        this.CHANGE_OPERATION_ID = value;
    }

    public String getCHANGE_OPERATION_TIMESTAMP() {
        return CHANGE_OPERATION_TIMESTAMP;
    }
    public void setCHANGE_OPERATION_TIMESTAMP(String value) {
        this.CHANGE_OPERATION_TIMESTAMP = value;
    }


    public String getTYPE_ID() {
        return TYPE_ID;
    }
    public void setTYPE_ID(String value) {
        this.TYPE_ID = value;
    }


    public String getSTATUS_ID() {
        return STATUS_ID;
    }
    public void setSTATUS_ID(String value) {
        this.STATUS_ID = value;
    }


    public String getCLASS_ID() {
        return CLASS_ID;
    }
    public void setCLASS_ID(String value) {
        this.CLASS_ID = value;
    }


    public String getSEGMENTATION_ID() {
        return SEGMENTATION_ID;
    }
    public void setSEGMENTATION_ID(String value) {
        this.SEGMENTATION_ID = value;
    }


    public String getCUSTOMER_SINCE() {
        return CUSTOMER_SINCE;
    }
    public void setCUSTOMER_SINCE(String value) {
        this.CUSTOMER_SINCE = value;
    }


    public String getEXEMPTION_CODE_ID() {
        return EXEMPTION_CODE_ID;
    }
    public void setEXEMPTION_CODE_ID(String value) {
        this.EXEMPTION_CODE_ID = value;
    }


    public String getEXEMPTION_SERIAL_NUMBER() {
        return EXEMPTION_SERIAL_NUMBER;
    }
    public void setEXEMPTION_SERIAL_NUMBER(String value) {
        this.EXEMPTION_SERIAL_NUMBER = value;
    }


    public String getEXEMPTION_FROM() {
        return EXEMPTION_FROM;
    }
    public void setEXEMPTION_FROM(String value) {
        this.EXEMPTION_FROM = value;
    }


    public String getREFERENCE_TYPE_ID() {
        return REFERENCE_TYPE_ID;
    }
    public void setREFERENCE_TYPE_ID(String value) {
        this.REFERENCE_TYPE_ID = value;
    }


    public String getIDENTIFICATION_ID() {
        return IDENTIFICATION_ID;
    }
    public void setIDENTIFICATION_ID(String value) {
        this.IDENTIFICATION_ID = value;
    }


    public String getINTERNET_PASSWORD_ENC() {
        return INTERNET_PASSWORD_ENC;
    }
    public void setINTERNET_PASSWORD_ENC(String value) {
        this.INTERNET_PASSWORD_ENC = value;
    }


    public String getINTERNET_PASSWORD_IV() {
        return INTERNET_PASSWORD_IV;
    }
    public void setINTERNET_PASSWORD_IV(String value) {
        this.INTERNET_PASSWORD_IV = value;
    }


    public String getINTERNET_USER_NAME() {
        return INTERNET_USER_NAME;
    }
    public void setINTERNET_USER_NAME(String value) {
        this.INTERNET_USER_NAME = value;
    }


    public String getLANGUAGE_ID() {
        return LANGUAGE_ID;
    }
    public void setLANGUAGE_ID(String value) {
        this.LANGUAGE_ID = value;
    }


    public String getBIRTH_DATE() {
        return BIRTH_DATE;
    }
    public void setBIRTH_DATE(String value) {
        this.BIRTH_DATE = value;
    }


    public String getBUSINESS_UNIT_ID() {
        return BUSINESS_UNIT_ID;
    }
    public void setBUSINESS_UNIT_ID(String value) {
        this.BUSINESS_UNIT_ID = value;
    }


    public String getMAGAZINES() {
        return MAGAZINES;
    }
    public void setMAGAZINES(String value) {
        this.MAGAZINES = value;
    }


    public String getCOUNTRY_ID() {
        return COUNTRY_ID;
    }
    public void setCOUNTRY_ID(String value) {
        this.COUNTRY_ID = value;
    }
}
