package Clases;

import com.fasterxml.jackson.annotation.JsonIgnoreProperties;
import com.fasterxml.jackson.annotation.JsonProperty;

@JsonIgnoreProperties(ignoreUnknown = true)
public class Stg_Customer_RF { //implements org.apache.avro.specific.SpecificRecord
    @JsonProperty("op_type")
    private String op_type;
    @JsonProperty("op_ts")
    private String op_ts;
    @JsonProperty("current_ts")
    private String current_ts;
    @JsonProperty("pos")
    private String pos;


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



    @JsonProperty("CLASS_KEY")
    private String CLASS_KEY;
    @JsonProperty("STATUS_KEY")
    private String STATUS_KEY;
    @JsonProperty("TYPE_KEY")
    private String TYPE_KEY;
    @JsonProperty("REFERENCE_TYPE_KEY")
    private String REFERENCE_TYPE_KEY;

    @JsonProperty("CLASS_NAME")
    private String CLASS_NAME;

    @JsonProperty("STATUS_NAME")
    private String STATUS_NAME;

    @JsonProperty("TYPE_NAME")
    private String TYPE_NAME;

    @JsonProperty("REFERENCE_TYPE_NAME")
    private String REFERENCE_TYPE_NAME;

    public <GV> Stg_Customer_RF(Stg_Customer customer, GV refclass) {
    }

    public Stg_Customer_RF() {

    }
/*
    public <GV> Stg_Customer_RF() {

    }
 */

/*
    public Stg_Customer_RF(Stg_Customer customer, RefCustomerClass refclass) {
        this.setOp_type(customer.getOp_type());
        this.setOp_ts(customer.getOp_ts());
        this.setCurrent_ts(customer.getCurrent_ts());
        this.setPos(customer.getPos());
        this.setId(customer.getAfter().getId());
        this.setSOURCE_ID(customer.getAfter().getSOURCE_ID());
        this.setENTITY_ID(customer.getAfter().getENTITY_ID());
        this.setCHANGE_OPERATION_TIMESTAMP(customer.getAfter().getCHANGE_OPERATION_TIMESTAMP());
        this.setCHANGE_OPERATION_ID(customer.getAfter().getCHANGE_OPERATION_ID());
        this.setTYPE_ID(customer.getAfter().getTYPE_ID());
        this.setSTATUS_ID(customer.getAfter().getSTATUS_ID());
        this.setCLASS_ID(customer.getAfter().getCLASS_ID());
        this.setSEGMENTATION_ID(customer.getAfter().getSEGMENTATION_ID());
        this.setCUSTOMER_SINCE(customer.getAfter().getCUSTOMER_SINCE());
        this.setEXEMPTION_CODE_ID(customer.getAfter().getEXEMPTION_CODE_ID());
        this.setEXEMPTION_SERIAL_NUMBER(customer.getAfter().getEXEMPTION_SERIAL_NUMBER());
        this.setEXEMPTION_FROM(customer.getAfter().getEXEMPTION_FROM());
        this.setREFERENCE_TYPE_ID(customer.getAfter().getREFERENCE_TYPE_ID());
        this.setIDENTIFICATION_ID(customer.getAfter().getIDENTIFICATION_ID());
        this.setINTERNET_PASSWORD_ENC(customer.getAfter().getINTERNET_PASSWORD_ENC());
        this.setINTERNET_PASSWORD_IV(customer.getAfter().getINTERNET_PASSWORD_IV());
        this.setINTERNET_USER_NAME(customer.getAfter().getINTERNET_USER_NAME());
        this.setLANGUAGE_ID(customer.getAfter().getLANGUAGE_ID());
        this.setBIRTH_DATE(customer.getAfter().getBIRTH_DATE());
        this.setBUSINESS_UNIT_ID(customer.getAfter().getBUSINESS_UNIT_ID());
        this.setMAGAZINES(customer.getAfter().getMAGAZINES());
        this.setCOUNTRY_ID(customer.getAfter().getCOUNTRY_ID());
        if (refclass!= null) {
            this.setCLASS_NAME(refclass.getNAME());
        }
        else
        {
            this.setCLASS_NAME("*NOT_FOUND*");
        }
    }*/
/*
    public <RV> Stg_Customer_RF(Stg_Customer customer, RefCustomerClass refclass) {
        this.setOp_type(customer.getOp_type());
        this.setOp_ts(customer.getOp_ts());
        this.setCurrent_ts(customer.getCurrent_ts());
        this.setPos(customer.getPos());
        this.setId(customer.getAfter().getId());
        this.setSOURCE_ID(customer.getAfter().getSOURCE_ID());
        this.setENTITY_ID(customer.getAfter().getENTITY_ID());
        this.setCHANGE_OPERATION_TIMESTAMP(customer.getAfter().getCHANGE_OPERATION_TIMESTAMP());
        this.setCHANGE_OPERATION_ID(customer.getAfter().getCHANGE_OPERATION_ID());
        this.setTYPE_ID(customer.getAfter().getTYPE_ID());
        this.setSTATUS_ID(customer.getAfter().getSTATUS_ID());
        this.setCLASS_ID(customer.getAfter().getCLASS_ID());
        this.setSEGMENTATION_ID(customer.getAfter().getSEGMENTATION_ID());
        this.setCUSTOMER_SINCE(customer.getAfter().getCUSTOMER_SINCE());
        this.setEXEMPTION_CODE_ID(customer.getAfter().getEXEMPTION_CODE_ID());
        this.setEXEMPTION_SERIAL_NUMBER(customer.getAfter().getEXEMPTION_SERIAL_NUMBER());
        this.setEXEMPTION_FROM(customer.getAfter().getEXEMPTION_FROM());
        this.setREFERENCE_TYPE_ID(customer.getAfter().getREFERENCE_TYPE_ID());
        this.setIDENTIFICATION_ID(customer.getAfter().getIDENTIFICATION_ID());
        this.setINTERNET_PASSWORD_ENC(customer.getAfter().getINTERNET_PASSWORD_ENC());
        this.setINTERNET_PASSWORD_IV(customer.getAfter().getINTERNET_PASSWORD_IV());
        this.setINTERNET_USER_NAME(customer.getAfter().getINTERNET_USER_NAME());
        this.setLANGUAGE_ID(customer.getAfter().getLANGUAGE_ID());
        this.setBIRTH_DATE(customer.getAfter().getBIRTH_DATE());
        this.setBUSINESS_UNIT_ID(customer.getAfter().getBUSINESS_UNIT_ID());
        this.setMAGAZINES(customer.getAfter().getMAGAZINES());
        this.setCOUNTRY_ID(customer.getAfter().getCOUNTRY_ID());
        if (refclass!= null) {
            this.setCLASS_NAME(((RefCustomerClass)refclass).getNAME());
        }
        else
        {
            this.setCLASS_NAME("*NOT_FOUND*");
        }
    }
*/

    public Stg_Customer_RF(Stg_Customer customer) {
        this.setOp_type(customer.getOp_type());
        this.setOp_ts(customer.getOp_ts());
        this.setCurrent_ts(customer.getCurrent_ts());
        this.setPos(customer.getPos());
        this.setId(customer.getAfter().getId());
        this.setSOURCE_ID(customer.getAfter().getSOURCE_ID());
        this.setENTITY_ID(customer.getAfter().getENTITY_ID());
        this.setCHANGE_OPERATION_TIMESTAMP(customer.getAfter().getCHANGE_OPERATION_TIMESTAMP());
        this.setCHANGE_OPERATION_ID(customer.getAfter().getCHANGE_OPERATION_ID());
        this.setTYPE_ID(customer.getAfter().getTYPE_ID());
        this.setSTATUS_ID(customer.getAfter().getSTATUS_ID());
        this.setCLASS_ID(customer.getAfter().getCLASS_ID());
        this.setSEGMENTATION_ID(customer.getAfter().getSEGMENTATION_ID());
        this.setCUSTOMER_SINCE(customer.getAfter().getCUSTOMER_SINCE());
        this.setEXEMPTION_CODE_ID(customer.getAfter().getEXEMPTION_CODE_ID());
        this.setEXEMPTION_SERIAL_NUMBER(customer.getAfter().getEXEMPTION_SERIAL_NUMBER());
        this.setEXEMPTION_FROM(customer.getAfter().getEXEMPTION_FROM());
        this.setREFERENCE_TYPE_ID(customer.getAfter().getREFERENCE_TYPE_ID());
        this.setIDENTIFICATION_ID(customer.getAfter().getIDENTIFICATION_ID());
        this.setINTERNET_PASSWORD_ENC(customer.getAfter().getINTERNET_PASSWORD_ENC());
        this.setINTERNET_PASSWORD_IV(customer.getAfter().getINTERNET_PASSWORD_IV());
        this.setINTERNET_USER_NAME(customer.getAfter().getINTERNET_USER_NAME());
        this.setLANGUAGE_ID(customer.getAfter().getLANGUAGE_ID());
        this.setBIRTH_DATE(customer.getAfter().getBIRTH_DATE());
        this.setBUSINESS_UNIT_ID(customer.getAfter().getBUSINESS_UNIT_ID());
        this.setMAGAZINES(customer.getAfter().getMAGAZINES());
        this.setCOUNTRY_ID(customer.getAfter().getCOUNTRY_ID());

        this.setCLASS_KEY(customer.getAfter().getCLASS_ID() + "_" + customer.getAfter().getSOURCE_ID());
        this.setSTATUS_KEY(customer.getAfter().getSTATUS_ID() + "_" + customer.getAfter().getSOURCE_ID());
        this.setTYPE_KEY(customer.getAfter().getTYPE_ID() + "_" + customer.getAfter().getSOURCE_ID());
        this.setREFERENCE_TYPE_KEY(customer.getAfter().getSEGMENTATION_ID() + "_" + customer.getAfter().getSOURCE_ID());

        //this.setCLASS_NAME("");
    }

    public Stg_Customer_RF(Stg_Customer_RF customer, RefCustomerClass refclass) {
        this.setOp_type(customer.getOp_type());
        this.setOp_ts(customer.getOp_ts());
        this.setCurrent_ts(customer.getCurrent_ts());
        this.setPos(customer.getPos());
        this.setId(customer.getId());
        this.setSOURCE_ID(customer.getSOURCE_ID());
        this.setENTITY_ID(customer.getENTITY_ID());
        this.setCHANGE_OPERATION_TIMESTAMP(customer.getCHANGE_OPERATION_TIMESTAMP());
        this.setCHANGE_OPERATION_ID(customer.getCHANGE_OPERATION_ID());
        this.setTYPE_ID(customer.getTYPE_ID());
        this.setSTATUS_ID(customer.getSTATUS_ID());
        this.setCLASS_ID(customer.getCLASS_ID());
        this.setSEGMENTATION_ID(customer.getSEGMENTATION_ID());
        this.setCUSTOMER_SINCE(customer.getCUSTOMER_SINCE());
        this.setEXEMPTION_CODE_ID(customer.getEXEMPTION_CODE_ID());
        this.setEXEMPTION_SERIAL_NUMBER(customer.getEXEMPTION_SERIAL_NUMBER());
        this.setEXEMPTION_FROM(customer.getEXEMPTION_FROM());
        this.setREFERENCE_TYPE_ID(customer.getREFERENCE_TYPE_ID());
        this.setIDENTIFICATION_ID(customer.getIDENTIFICATION_ID());
        this.setINTERNET_PASSWORD_ENC(customer.getINTERNET_PASSWORD_ENC());
        this.setINTERNET_PASSWORD_IV(customer.getINTERNET_PASSWORD_IV());
        this.setINTERNET_USER_NAME(customer.getINTERNET_USER_NAME());
        this.setLANGUAGE_ID(customer.getLANGUAGE_ID());
        this.setBIRTH_DATE(customer.getBIRTH_DATE());
        this.setBUSINESS_UNIT_ID(customer.getBUSINESS_UNIT_ID());
        this.setMAGAZINES(customer.getMAGAZINES());
        this.setCOUNTRY_ID(customer.getCOUNTRY_ID());
        if (refclass!= null) {
            this.setCLASS_NAME(refclass.getNAME());
        }
        else
        {
            this.setCLASS_NAME("*NOT_FOUND*");
        }
    }

    public Stg_Customer_RF(Stg_Customer_RF customer, RefCustomerStatus refstatus) {
        if (refstatus!= null) {
            this.setSTATUS_NAME(refstatus.getNAME());
        }
        else
        {
            this.setSTATUS_NAME("*NOT_FOUND*");
        }
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




    public String getCLASS_NAME() {
        return CLASS_NAME;
    }
    public void setCLASS_NAME(String value) {
        this.CLASS_NAME = value;
    }

    public String getSTATUS_NAME() {
        return STATUS_NAME;
    }
    public void setSTATUS_NAME(String value) {
        this.STATUS_NAME = value;
    }

    public String getTYPE_NAME() {
        return TYPE_NAME;
    }
    public void setTYPE_NAME(String value) {
        this.TYPE_NAME = value;
    }

    public String getREFERENCE_TYPE_NAME() {
        return REFERENCE_TYPE_NAME;
    }
    public void setREFERENCE_TYPE_NAME(String value) {
        this.REFERENCE_TYPE_NAME = value;
    }


    public String getCLASS_KEY() {
        return CLASS_KEY;
    }
    public void setCLASS_KEY(String value) {
        this.CLASS_KEY = value;
    }


    public String getSTATUS_KEY() {
        return STATUS_KEY;
    }
    public void setSTATUS_KEY(String value) {
        this.STATUS_KEY = value;
    }


    public String getTYPE_KEY() {
        return TYPE_KEY;
    }
    public void setTYPE_KEY(String value) {
        this.TYPE_KEY = value;
    }


    public String getREFERENCE_TYPE_KEY() {
        return REFERENCE_TYPE_KEY;
    }
    public void setREFERENCE_TYPE_KEY(String value) {
        this.REFERENCE_TYPE_KEY = value;
    }

}
