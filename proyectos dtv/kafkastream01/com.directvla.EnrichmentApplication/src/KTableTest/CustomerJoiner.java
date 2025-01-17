package KTableTest;

import Clases.RefCustomerClass;
import Clases.Stg_Customer;
import Clases.Stg_Customer_RF;
import org.apache.kafka.streams.kstream.ValueJoiner;

public class CustomerJoiner implements ValueJoiner<Stg_Customer, RefCustomerClass, Stg_Customer_RF> {


    public Stg_Customer_RF apply(Stg_Customer customer, RefCustomerClass ref) {
        /*return Stg_Customer_RF.newBuilder()
                .setId(ref.getNAME())
                .setTitle(customer.get())
                .setReleaseYear(customer.getReleaseYear())
                .setRating(customer.getRating())
                .build();
         */

        Stg_Customer_RF newCustomer = new Stg_Customer_RF();
        newCustomer.setOp_type(customer.getOp_type());
        newCustomer.setOp_ts(customer.getOp_ts());
        newCustomer.setCurrent_ts(customer.getCurrent_ts());
        newCustomer.setPos(customer.getPos());
        newCustomer.setId(customer.getAfter().getId());
        newCustomer.setSOURCE_ID(customer.getAfter().getSOURCE_ID());
        newCustomer.setENTITY_ID(customer.getAfter().getENTITY_ID());
        newCustomer.setCHANGE_OPERATION_TIMESTAMP(customer.getAfter().getCHANGE_OPERATION_TIMESTAMP());
        newCustomer.setCHANGE_OPERATION_ID(customer.getAfter().getCHANGE_OPERATION_ID());
        newCustomer.setTYPE_ID(customer.getAfter().getTYPE_ID());
        newCustomer.setSTATUS_ID(customer.getAfter().getSTATUS_ID());
        newCustomer.setCLASS_ID(customer.getAfter().getCLASS_ID());
        newCustomer.setSEGMENTATION_ID(customer.getAfter().getSEGMENTATION_ID());
        newCustomer.setCUSTOMER_SINCE(customer.getAfter().getCUSTOMER_SINCE());
        newCustomer.setEXEMPTION_CODE_ID(customer.getAfter().getEXEMPTION_CODE_ID());
        newCustomer.setEXEMPTION_SERIAL_NUMBER(customer.getAfter().getEXEMPTION_SERIAL_NUMBER());
        newCustomer.setEXEMPTION_FROM(customer.getAfter().getEXEMPTION_FROM());
        newCustomer.setREFERENCE_TYPE_ID(customer.getAfter().getREFERENCE_TYPE_ID());
        newCustomer.setIDENTIFICATION_ID(customer.getAfter().getIDENTIFICATION_ID());
        newCustomer.setINTERNET_PASSWORD_ENC(customer.getAfter().getINTERNET_PASSWORD_ENC());
        newCustomer.setINTERNET_PASSWORD_IV(customer.getAfter().getINTERNET_PASSWORD_IV());
        newCustomer.setINTERNET_USER_NAME(customer.getAfter().getINTERNET_USER_NAME());
        newCustomer.setLANGUAGE_ID(customer.getAfter().getLANGUAGE_ID());
        newCustomer.setBIRTH_DATE(customer.getAfter().getBIRTH_DATE());
        newCustomer.setBUSINESS_UNIT_ID(customer.getAfter().getBUSINESS_UNIT_ID());
        newCustomer.setMAGAZINES(customer.getAfter().getMAGAZINES());
        newCustomer.setCOUNTRY_ID(customer.getAfter().getCOUNTRY_ID());

        newCustomer.setCLASS_NAME(ref.getNAME());

        return newCustomer;
    }


    public Stg_Customer_RF apply(Stg_Customer customer) {
        /*return Stg_Customer_RF.newBuilder()
                .setId(ref.getNAME())
                .setTitle(customer.get())
                .setReleaseYear(customer.getReleaseYear())
                .setRating(customer.getRating())
                .build();
         */

        Stg_Customer_RF newCustomer = new Stg_Customer_RF();
        newCustomer.setOp_type(customer.getOp_type());
        newCustomer.setOp_ts(customer.getOp_ts());
        newCustomer.setCurrent_ts(customer.getCurrent_ts());
        newCustomer.setPos(customer.getPos());
        newCustomer.setId(customer.getAfter().getId());
        newCustomer.setSOURCE_ID(customer.getAfter().getSOURCE_ID());
        newCustomer.setENTITY_ID(customer.getAfter().getENTITY_ID());
        newCustomer.setCHANGE_OPERATION_TIMESTAMP(customer.getAfter().getCHANGE_OPERATION_TIMESTAMP());
        newCustomer.setCHANGE_OPERATION_ID(customer.getAfter().getCHANGE_OPERATION_ID());
        newCustomer.setTYPE_ID(customer.getAfter().getTYPE_ID());
        newCustomer.setSTATUS_ID(customer.getAfter().getSTATUS_ID());
        newCustomer.setCLASS_ID(customer.getAfter().getCLASS_ID());
        newCustomer.setSEGMENTATION_ID(customer.getAfter().getSEGMENTATION_ID());
        newCustomer.setCUSTOMER_SINCE(customer.getAfter().getCUSTOMER_SINCE());
        newCustomer.setEXEMPTION_CODE_ID(customer.getAfter().getEXEMPTION_CODE_ID());
        newCustomer.setEXEMPTION_SERIAL_NUMBER(customer.getAfter().getEXEMPTION_SERIAL_NUMBER());
        newCustomer.setEXEMPTION_FROM(customer.getAfter().getEXEMPTION_FROM());
        newCustomer.setREFERENCE_TYPE_ID(customer.getAfter().getREFERENCE_TYPE_ID());
        newCustomer.setIDENTIFICATION_ID(customer.getAfter().getIDENTIFICATION_ID());
        newCustomer.setINTERNET_PASSWORD_ENC(customer.getAfter().getINTERNET_PASSWORD_ENC());
        newCustomer.setINTERNET_PASSWORD_IV(customer.getAfter().getINTERNET_PASSWORD_IV());
        newCustomer.setINTERNET_USER_NAME(customer.getAfter().getINTERNET_USER_NAME());
        newCustomer.setLANGUAGE_ID(customer.getAfter().getLANGUAGE_ID());
        newCustomer.setBIRTH_DATE(customer.getAfter().getBIRTH_DATE());
        newCustomer.setBUSINESS_UNIT_ID(customer.getAfter().getBUSINESS_UNIT_ID());
        newCustomer.setMAGAZINES(customer.getAfter().getMAGAZINES());
        newCustomer.setCOUNTRY_ID(customer.getAfter().getCOUNTRY_ID());

        newCustomer.setCLASS_NAME("class_name");
        return newCustomer;
    }
}
