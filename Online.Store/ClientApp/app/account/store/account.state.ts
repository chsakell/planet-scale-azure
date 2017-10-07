import { RegisterVM } from "../../models/register-vm";
import { LoginVM } from "../../models/login-vm";

export interface AccountState {
    newUser: RegisterVM;
    loginUser: LoginVM;
};